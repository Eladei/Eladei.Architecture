using Confluent.Kafka;
using DotNetEnv;
using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.Architecture.Jobs.Quartz.Extensions;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka;
using Eladei.BookRating.Api.Configuration;
using Eladei.BookRating.Api.Filters;
using Eladei.BookRating.Api.Helpers;
using Eladei.BookRating.Api.Jobs;
using Eladei.BookRating.Api.Logging;
using Eladei.BookRating.Api.Policies;
using Eladei.BookRating.Infrastructure.Adapters;
using Eladei.BookRating.Infrastructure.Outbox;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Kafka;
using Rebus.Retry.Simple;
using Serilog;
using Serilog.Events;

namespace Eladei.BookRating.Api;

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

        appBuilder.Services.AddTransient<IOutboxMessageFactory, OutboxMessageFactory>();
        appBuilder.Services.AddTransient<IEfOutboxIntegrationEventWriter<BookRatingDbContext>, OutboxIntegrationEventWriter>();

        appBuilder.Services.AddTransient<IOperationExecutionPolicyProvider, OperationExecutionPolicyProvider>();

        appBuilder.Services.AddTransient<IEfCommandExecutor<BookRatingDbContext>, EfCommandExecutor<BookRatingDbContext>>();
        appBuilder.Services.AddTransient<IEfCommandExecutorLogger, EfCommandExecutorLogger>();
        appBuilder.Services.AddTransient<ICommandExecutor, EfCommandExecutorAdapter>();

        appBuilder.Services.AddTransient<IEfQueryExecutor<BookRatingDbContext>, EfQueryExecutor<BookRatingDbContext>>();
        appBuilder.Services.AddTransient<IEfQueryExecutorLogger, EfQueryExecutorLogger>();
        appBuilder.Services.AddTransient<IQueryExecutor, EfQueryExecutorAdapter>();
        appBuilder.Services.AddTransient<IOperationExecutor, OperationExecutor>();
    }

    private static void SetDbServices(IServiceCollection services)
    {
        string connectionStr = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.DbConnectionString);

        services.AddDbContextPool<BookRatingDbContext>(
            o => o.UseNpgsql(connectionStr));

        services.AddDbContextFactory<BookRatingDbContext>();
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

    private static void SetJobs(IServiceCollection services)
    {
        var integrationEventsSenderJobConfig = new OutboxIntegrationEventsSenderJobConfig
        {
            ReservingTimeInSeconds = EnvVariablesHelper.GetVariable<uint>(
                EnvVariablesNames.IntegrationEventsReservingTimeForSendingInSeconds),
            MaxEventsToReserve = EnvVariablesHelper.GetVariable<uint>(
                EnvVariablesNames.IntegrationEventsReservingCountForSending),
        };

        services.AddSingleton<OutboxIntegrationEventsSenderJobConfig>(integrationEventsSenderJobConfig);

        services.AddQuartz(q =>
        {
            var outOfDateServersCron = EnvVariablesHelper
                .GetVariable<string>(EnvVariablesNames.IntegrationEventsSenderJobCron);

            q.AddNewJob<OutboxIntegrationEventsSenderJob>(outOfDateServersCron);
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

    private static void SetUpEventBus(IServiceCollection services)
    {
        var host = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaHost);
        var port = EnvVariablesHelper.GetVariable<ushort>(EnvVariablesNames.KafkaPort);

        var kafkaEndpoint = $"{host}:{port}";

        var topic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaTopicForCurrentService);
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

            var handlerActivator = new BuiltinHandlerActivator();

            var kafkaLogger = provider.GetRequiredService<ILogger<KafkaEventBus>>();

            var kafkaBus = Configure.With(handlerActivator)
                .Logging(l => l.MicrosoftExtensionsLogging(kafkaLogger))
                .Transport(t => t.UseKafka(kafkaEndpoint, topic, producerConfig, consumerConfig))
                .Options(o =>
                {
                    o.SetMaxParallelism(1);
                    o.RetryStrategy(
                        errorQueueName: errorTopic,
                        maxDeliveryAttempts: integrationEventsHandlingRetriesCount);
                })
                .Start();

            return new KafkaEventBus(kafkaBus, topic, kafkaLogger);
        });
    }
}
