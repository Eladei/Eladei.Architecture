namespace Eladei.BookInfo.Api.Configuration;

public static class EnvVariablesNames
{
    public const string DbConnectionString = "DB_CONNECTION_STRING";

    public const string KafkaHost = "KAFKA_HOST";

    public const string KafkaPort = "KAFKA_PORT";

    public const string KafkaGroupIdCurrentService = "KAFKA_GROUP_ID_CURRENT_SERVICE";

    public const string KafkaTopicForCurrentService = "KAFKA_TOPIC_CURRENT_SERVICE";

    public const string KafkaErrorTopicForCurrentService = "KAFKA_ERROR_TOPIC_CURRENT_SERVICE";

    public const string KafkaTopicBookRatingService = "KAFKA_TOPIC_BOOK_RATING_SERVICE";

    public const string IntegrationEventsReservingTimeForSendingInSeconds =
        "INTEGRATION_EVENTS_RESERVING_TIME_FOR_SENDING_IN_SECONDS";

    public const string IntegrationEventsReservingCountForSending =
        "INTEGRATION_EVENTS_RESERVING_COUNT_FOR_SENDING";

    public const string IntegrationEventsHandlingRetriesCount =
        "INTEGRATION_EVENTS_HANDLING_RETRIES_COUNT";

    public const string IntegrationEventsSenderJobCron =
        "INTEGRATION_EVENTS_SENDER_JOB_CRON";
}
