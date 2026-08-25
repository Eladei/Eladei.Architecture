namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Integration event bus
/// </summary>
public interface IIntegrationEventBus
{
    /// <summary>
    /// Publishes an integration event
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    /// <param name="headers">Headers for event publishing</param>
    Task PublishEventAsync(IIntegrationEvent integrationEvent, Dictionary<string, string>? headers = null);

    /// <summary>
    /// Header names used for integration events
    /// </summary>
    public static class IntegrationEventHeadersNames
    {
        /// <summary>
        /// The name of the header that contains the event id
        /// </summary>
        public const string EventId = "event-id";

        /// <summary>
        /// The name of the header that contains the message key
        /// </summary>
        public const string MessageKey = "message-key";

        /// <summary>
        /// The name of the header that contains the correlation id
        /// </summary>
        public const string CorrelationId = "correlation-id";
    }
}
