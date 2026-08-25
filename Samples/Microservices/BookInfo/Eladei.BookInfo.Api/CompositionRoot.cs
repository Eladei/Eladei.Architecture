using Confluent.Kafka;
using DotNetEnv;
using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Api.Configuration;
using Eladei.BookInfo.Api.Extensions;
using Eladei.BookInfo.Api.Filters;
using Eladei.BookInfo.Api.Helpers;
using Eladei.BookInfo.Api.IntegrationEvents.Handlers;
using Eladei.BookInfo.Api.Logging;
using Eladei.BookInfo.Api.Policies;
using Eladei.BookInfo.Api.Services;
using Eladei.BookInfo.Infrastructure.Adapters;
using Eladei.BookInfo.Infrastructure.Outbox;
using Eladei.BookInfo.Model;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Extensions;
using Rebus.Kafka;
using Rebus.Retry.Simple;
using Serilog;
using Serilog.Events;

namespace Eladei.BookInfo.Api;

public static class CompositionRoot
{
    public static void DefineDependencies(WebApplicationBuilder appBuilder)
    {
        Env.Load();

        appBuilder.Services.AddAuthorization();

        SetDbServices(appBuilder.Services);

        SetLoggers(appBuilder);

        SetInterceptors(appBuilder.Services);

        SetJobs(appBuilder.Services);

        SetUpEventBus(appBuilder.Services);

        appBuilder.Services.AddGrpcReflection();

        appBuilder.Services.AddTransient<IOperationExecutionPolicyProvider, OperationExecutionPolicyProvider>();
        appBuilder.Services.AddTransient<IEfOutboxIntegrationEventWriter<BookInfoDbContext>, MockOutboxIntegrationEventWriter>();

        appBuilder.Services.AddTransient<ICommandExecutor, EfCommandExecutorAdapter>();
        appBuilder.Services.AddTransient<IEfCommandExecutorLogger, EfCommandExecutorLogger>();
        appBuilder.Services.AddTransient<IEfCommandExecutor<BookInfoDbContext>, EfCommandExecutor<BookInfoDbContext>>();

        appBuilder.Services.AddTransient<IQueryExecutor, EfQueryExecutorAdapter>();
        appBuilder.Services.AddTransient<IEfQueryExecutorLogger, EfQueryExecutorLogger>();
        appBuilder.Services.AddTransient<IEfQueryExecutor<BookInfoDbContext>, EfQueryExecutor<BookInfoDbContext>>();
        appBuilder.Services.AddTransient<IOperationExecutor, OperationExecutor>();

        appBuilder.Services.AddHostedService<EventBusStarter>();
    }

    private static void SetDbServices(IServiceCollection services)
    {
        string connectionStr = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.DbConnectionString);

        services.AddDbContextPool<BookInfoDbContext>(
            o => o.UseNpgsql(connectionStr));

        services.AddDbContextFactory<BookInfoDbContext>();
    }

    private static void SetLoggers(WebApplicationBuilder appBuilder)
    {
        const string consoleOutputTemplate = "[{Timestamp:u} {Level}] [{CorrelationId}] {Message}{NewLine}{Exception}";
        const string fileOutputTemplate = consoleOutputTemplate;
        const string outputLogFile = "logs/log-.txt";

        // Setting up Serilog
        appBuilder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .MinimumLevel.Information() // Default logging level
            .Enrich.WithCorrelationId()
            .Enrich.WithCorrelationIdHeader()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal) // Ignore Microsoft logs
            .MinimumLevel.Override("System", LogEventLevel.Fatal) // Ignore System logs
            .WriteTo.Console(outputTemplate: consoleOutputTemplate) // Console format
            .WriteTo.File(outputLogFile,
                rollingInterval: RollingInterval.Day,
                outputTemplate: fileOutputTemplate)); // File format

        appBuilder.Services.AddTransient<ICorrelationContext, CorrelationContext>();
    }

    private static void SetInterceptors(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<CorrelationIdInterceptor>();
            options.Interceptors.Add<LoggerInterceptor>();
            options.Interceptors.Add<ErrorInterceptor>();
        });
    }

    private static void SetJobs(IServiceCollection services) { }

    private static void SetUpEventBus(IServiceCollection services)
    {
        var host = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaHost);
        var port = EnvVariablesHelper.GetVariable<ushort>(EnvVariablesNames.KafkaPort);

        var kafkaEndpoint = $"{host}:{port}";

        var bookRatingTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaTopicBookRatingService);
        var currentServiceTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaTopicForCurrentService);
        var errorTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaErrorTopicForCurrentService);

        var groupId = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaGroupIdCurrentService);

        var integrationEventsHandlingRetriesCount = EnvVariablesHelper
            .GetVariable<int>(EnvVariablesNames.IntegrationEventsHandlingRetriesCount);

        services.AddSingleton<IIntegrationEventBus>(provider =>
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = host,

                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 3000,
                SocketTimeoutMs = 3000,

                MessageSendMaxRetries = 0,

                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 1000,

                AllowAutoCreateTopics = true, // In production, avoid this option and create topics in advance
            };

            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = host,
                GroupId = groupId,
                EnableAutoCommit = false,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true,

                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 1000,

                AllowAutoCreateTopics = true, // In production, avoid this option and create topics in advance
            };

            var handlersFactory = new KafkaEventHandlerFactory(provider);

            var handlerActivator = new BuiltinHandlerActivator()
                .Register(c =>
                {
                    var metadata = c.Headers.ExtractMetadata();

                    return handlersFactory.CreateHandler<
                        BookWasRegisteredInRatingIntegrationEventHandler,
                        BookWasRegisteredInRatingIntegrationEvent>(metadata, c.GetCancellationToken());
                })
                .Register(c =>
                {
                    var metadata = c.Headers.ExtractMetadata();

                    return handlersFactory.CreateHandler<
                        BookInfoWasUpdatedInRatingIntegrationEventHandler,
                        BookInfoWasUpdatedInRatingIntegrationEvent>(metadata, c.GetCancellationToken());
                })
                .Register(c =>
                {
                    var metadata = c.Headers.ExtractMetadata();

                    return handlersFactory.CreateHandler<
                        BookWasRemovedFromRatingIntegrationEventHandler,
                        BookWasRemovedFromRatingIntegrationEvent>(metadata, c.GetCancellationToken());
                });

            var kafkaLogger = provider.GetRequiredService<ILogger<KafkaEventBus>>();

            var kafkaBus = Configure.With(handlerActivator)
                .Logging(l => l.MicrosoftExtensionsLogging(kafkaLogger))
                .Transport(t => t.UseKafka(kafkaEndpoint, currentServiceTopic, producerConfig, consumerConfig))
                .Options(o =>
                {
                    o.SetMaxParallelism(1);
                    o.RetryStrategy(
                        errorQueueName: errorTopic,
                        maxDeliveryAttempts: integrationEventsHandlingRetriesCount);
                })
                .Start();

            kafkaBus.Advanced.Topics.Subscribe(bookRatingTopic);

            return new KafkaEventBus(kafkaBus, currentServiceTopic, kafkaLogger);
        });
    }
}
