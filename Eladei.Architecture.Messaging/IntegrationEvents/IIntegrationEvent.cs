namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Integration event
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Event identifier
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Identifier of the entity associated with the event
    /// </summary>
    Guid EntityId { get; }

    /// <summary>
    /// Correlation identifier used for end-to-end tracing
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// UTC timestamp when the event was created
    /// </summary>
    DateTime CreatedOnUtc { get; }
}