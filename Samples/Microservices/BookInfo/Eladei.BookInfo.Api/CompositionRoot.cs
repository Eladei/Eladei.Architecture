using Confluent.Kafka;
using DotNetEnv;
using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka.Extensions;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka.Interceptors;
using Eladei.BookInfo.Api.Configuration;
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

/// <summary>
/// Composition root of the service
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Defines the dependencies of the service
    /// </summary>
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

        appBuilder.Services.AddTransient<IOperationExecutionPolicyService, OperationExecutionPolicyService>();
        appBuilder.Services.AddTransient<IEfOutboxDomainEventDao<BookInfoDbContext>, MockOutboxDomainEventDao>();

        appBuilder.Services.AddTransient<ICommandExecutor, EfCommandExecutorAdapter>();
        appBuilder.Services.AddTransient<IEfCommandExecutorLogger, EfCommandExecutorLogger>();
        appBuilder.Services.AddTransient<IEfCommandExecutor<BookInfoDbContext>, EfCommandExecutor<BookInfoDbContext>>();

        appBuilder.Services.AddTransient<IQueryExecutor, EfQueryExecutorAdapter>();
        appBuilder.Services.AddTransient<IEfQueryExecutorLogger, EfQueryExecutorLogger>();
        appBuilder.Services.AddTransient<IEfQueryExecutor<BookInfoDbContext>, EfQueryExecutor<BookInfoDbContext>>();
        appBuilder.Services.AddTransient<IOperationExecutor, OperationExecutor>();

        appBuilder.Services.AddHostedService<EventBusStarter>();
    }

    /// <summary>
    /// Sets up the objects for working with the database
    /// </summary>
    /// <param name="services">Service collection</param>
    private static void SetDbServices(IServiceCollection services)
    {
        string connectionStr = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.DbConnectionString);

        services.AddDbContextPool<BookInfoDbContext>(
            o => o.UseNpgsql(connectionStr));

        services.AddDbContextFactory<BookInfoDbContext>();
    }

    /// <summary>
    /// Sets up the loggers
    /// </summary>
    /// <param name="appBuilder">Service builder</param>
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

    /// <summary>
    /// Sets up the interceptors
    /// </summary>
    /// <param name="services">Service collection</param>
    private static void SetInterceptors(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<CorrelationIdInterceptor>();
            options.Interceptors.Add<LoggerInterceptor>();
            options.Interceptors.Add<ErrorInterceptor>();
        });
    }

    /// <summary>
    /// Sets up the jobs
    /// </summary>
    /// <param name="services">Service collection</param>
    private static void SetJobs(IServiceCollection services) { }

    /// <summary>
    /// Configures the event bus
    /// </summary>
    /// <param name="services">Services of the current polling service</param>
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
                .Register(c => handlersFactory.CreateHandler<BookWasRegisteredInRatingIntegrationEventHandler, BookWasRegisteredInRatingIntegrationEvent>(c.GetCancellationToken()))
                .Register(c => handlersFactory.CreateHandler<BookInfoWasUpdatedInRatingIntegrationEventHandler, BookInfoWasUpdatedInRatingIntegrationEvent>(c.GetCancellationToken()))
                .Register(c => handlersFactory.CreateHandler<BookWasRemovedFromRatingIntegrationEventHandler, BookWasRemovedFromRatingIntegrationEvent>(c.GetCancellationToken()));

            var kafkaLogger = provider.GetRequiredService<ILogger<KafkaEventBus>>();

            var kafkaBus = Configure.With(handlerActivator)
                .Logging(l => l.MicrosoftExtensionsLogging(kafkaLogger))
                .Transport(t => t.UseKafka(kafkaEndpoint, currentServiceTopic, producerConfig, consumerConfig))
                .Options(o =>
                {
                    o.SetMaxParallelism(1);
                    o.InsertStepAfterAutoHeadersOutgoingStep(new AddKafkaKeyHeaderByEventIdStepInterceptor());
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