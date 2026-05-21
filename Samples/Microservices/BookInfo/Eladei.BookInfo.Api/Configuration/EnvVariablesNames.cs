namespace Eladei.BookInfo.Api.Configuration;

/// <summary>
/// Environment variables
/// </summary>
public static class EnvVariablesNames
{
    /// <summary>
    /// Database connection string
    /// </summary>
    public const string DbConnectionString = "DB_CONNECTION_STRING";

    /// <summary>
    /// Kafka broker host.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaHost = "KAFKA_HOST";

    /// <summary>
    /// Kafka broker port.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaPort = "KAFKA_PORT";

    /// <summary>
    /// Kafka consumer group id for the current microservice.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaGroupIdCurrentService = "KAFKA_GROUP_ID_CURRENT_SERVICE";

    /// <summary>
    /// Kafka topic for the current microservice.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaTopicForCurrentService = "KAFKA_TOPIC_CURRENT_SERVICE";

    /// <summary>
    /// Kafka error topic for the current microservice.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaErrorTopicForCurrentService = "KAFKA_ERROR_TOPIC_CURRENT_SERVICE";

    /// <summary>
    /// Kafka topic for Book Rating service.
    /// Used for message consumption configuration.
    /// </summary>
    public const string KafkaTopicBookRatingService = "KAFKA_TOPIC_BOOK_RATING_SERVICE";

    /// <summary>
    /// Time window (in seconds) for reserving integration events for sending.
    /// </summary>
    public const string IntegrationEventsReservingTimeForSendingInSeconds =
        "INTEGRATION_EVENTS_RESERVING_TIME_FOR_SENDING_IN_SECONDS";

    /// <summary>
    /// Number of integration events to reserve for sending.
    /// </summary>
    public const string IntegrationEventsReservingCountForSending =
        "INTEGRATION_EVENTS_RESERVING_COUNT_FOR_SENDING";

    /// <summary>
    /// Number of retry attempts for processing integration events
    /// before they are moved to the error queue.
    /// </summary>
    public const string IntegrationEventsHandlingRetriesCount =
        "INTEGRATION_EVENTS_HANDLING_RETRIES_COUNT";

    /// <summary>
    /// Cron expression for scheduling the integration events sender job.
    /// </summary>
    public const string IntegrationEventsSenderJobCron =
        "INTEGRATION_EVENTS_SENDER_JOB_CRON";
}