namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Metadata for Kafka integration events
/// </summary>
public record KafkaIntegrationEventMetadata
{
    /// <summary>
    /// Message key for Kafka
    /// </summary>
    public Guid? MessageKey { get; init; }

    /// <summary>
    /// The id of the integration event
    /// </summary>
    public Guid? EventId { get; init; }

    /// <summary>
    /// Correlation id for the integration event. 
    /// It used to correlate the integration event with the command that generated it.
    /// </summary>
    public Guid CorrelationId { get; init; }
}
