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
/// Корень сборки сервиса
/// </summary>
public static class CompositionRoot {
    /// <summary>
    /// Определяет зависимости сервиса
    /// </summary>
    public static void DefineDependencies(WebApplicationBuilder appBuilder) {
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
    /// Устанавливаем объекты для работы с БД
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private static void SetDbServices(IServiceCollection services) {
        string connectionStr = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.DbConnectionString);

        services.AddDbContextPool<BookInfoDbContext>(
            o => o.UseNpgsql(connectionStr));

        services.AddDbContextFactory<BookInfoDbContext>();
    }

    /// <summary>
    /// Устанавливает перехватчики
    /// </summary>
    /// <param name="appBuilder">Строитель сервиса</param>
    private static void SetLoggers(WebApplicationBuilder appBuilder) {
        const string consoleOutputTemplate = "[{Timestamp:u} {Level}] [{CorrelationId}] {Message}{NewLine}{Exception}";
        const string fileOutputTemplate = consoleOutputTemplate;
        const string outputLogFile = "logs/log-.txt";

        // Настройка Serilog
        appBuilder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .MinimumLevel.Information() // Уровень логирования по умолчанию
            .Enrich.WithCorrelationId()
            .Enrich.WithCorrelationIdHeader()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal) // Игнорировать логи Microsoft
            .MinimumLevel.Override("System", LogEventLevel.Fatal) // Игнорировать логи System
            .WriteTo.Console(outputTemplate: consoleOutputTemplate) // Формат для консоли
            .WriteTo.File(outputLogFile,
                rollingInterval: RollingInterval.Day,
                outputTemplate: fileOutputTemplate)); // Формат для файла

        appBuilder.Services.AddTransient<ICorrelationContext, CorrelationContext>();
    }

    /// <summary>
    /// Устанавливаем перехватчики
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private static void SetInterceptors(IServiceCollection services) {
        services.AddGrpc(options => {
            options.Interceptors.Add<CorrelationIdInterceptor>();
            options.Interceptors.Add<LoggerInterceptor>();
            options.Interceptors.Add<ErrorInterceptor>();
        });
    }

    /// <summary>
    /// Устанавливает jobs
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private static void SetJobs(IServiceCollection services) { }

    /// <summary>
    /// Конфигурирует шину событий
    /// </summary>
    /// <param name="services">Службы текущего сервиса опроса</param>
    private static void SetUpEventBus(IServiceCollection services) {
        var host = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaHost);
        var port = EnvVariablesHelper.GetVariable<ushort>(EnvVariablesNames.KafkaPort);

        var kafkaEndpoint = $"{host}:{port}";

        var bookRatingTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaTopicBookRatingService);
        var currentServiceTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaTopicForCurrentService);
        var errorTopic = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaErrorTopicForCurrentService);

        var groupId = EnvVariablesHelper.GetVariable<string>(EnvVariablesNames.KafkaGroupIdCurrentService);

        var integrationEventsHandlingRetriesCount = EnvVariablesHelper
            .GetVariable<int>(EnvVariablesNames.IntegrationEventsHandlingRetriesCount);

        services.AddSingleton<IIntegrationEventBus>(provider => {
            var producerConfig = new ProducerConfig() {
                BootstrapServers = host,

                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 3000,
                SocketTimeoutMs = 3000,

                MessageSendMaxRetries = 0,

                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 1000,

                AllowAutoCreateTopics = true, // В production избегать данной опции и создавать топики предварительно
            };

            var consumerConfig = new ConsumerConfig() {
                BootstrapServers = host,
                GroupId = groupId,
                EnableAutoCommit = false,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true,

                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 1000,

                AllowAutoCreateTopics = true, // В production избегать данной опции и создавать топики предварительно
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
                .Options(o => {
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