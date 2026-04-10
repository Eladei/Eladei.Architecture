namespace Eladei.BookInfo.Api.Configuration;

/// <summary>
/// Переменные окружения
/// </summary>
public static class EnvVariablesNames
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    public const string DbConnectionString = "DB_CONNECTION_STRING";

    /// <summary>
    /// Хост для подключения к шине Kafka.
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaHost = "KAFKA_HOST";

    /// <summary>
    /// Порт для подключения к шине Kafka.
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaPort = "KAFKA_PORT";

    /// <summary>
    /// Id группы текущего микросервиса в Kafka.
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaGroupIdCurrentService = "KAFKA_GROUP_ID_CURRENT_SERVICE";

    /// <summary>
    /// Топик текущего микросервиса.
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaTopicForCurrentService = "KAFKA_TOPIC_CURRENT_SERVICE";

    /// <summary>
    /// Топик ошибок текущего микросервиса.
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaErrorTopicForCurrentService = "KAFKA_ERROR_TOPIC_CURRENT_SERVICE";

    /// <summary>
    /// Топик рейтинга книг
    /// Используется для управления потреблением сообщений.
    /// </summary>
    public const string KafkaTopicBookRatingService = "KAFKA_TOPIC_BOOK_RATING_SERVICE";

    /// <summary>
    /// Время резервирования событий интеграции для отправки в секундах
    /// </summary>
    public const string IntegrationEventsReservingTimeForSendingInSeconds = "INTEGRATION_EVENTS_RESERVING_TIME_FOR_SENDING_IN_SECONDS";

    /// <summary>
    /// Время резервирования событий интеграции для отправки в секундах
    /// </summary>
    public const string IntegrationEventsReservingCountForSending = "INTEGRATION_EVENTS_RESERVING_COUNT_FOR_SENDING";

    /// <summary>
    /// Число попыток обработки событий интеграции, 
    /// прежде чем они будут помещены в очередь ошибок
    /// </summary>
    public const string IntegrationEventsHandlingRetriesCount = "INTEGRATION_EVENTS_HANDLING_RETRIES_COUNT";

    /// <summary>
    /// Периодичность запуска рассылки событий интеграции
    /// </summary>
    public const string IntegrationEventsSenderJobCron = "INTEGRATION_EVENTS_SENDER_JOB_CRON";
}