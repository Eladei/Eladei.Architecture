namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Integration event
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Constructor of the IntegrationEvent class
    /// </summary>
    /// <param name="entityId">The ID of the entity associated with the event</param>
    /// <param name="correlationId">The correlation ID for end-to-end tracing</param>
    public IntegrationEvent(Guid entityId, Guid correlationId)
    {
        EventId = Guid.NewGuid();
        EntityId = entityId;
        CorrelationId = correlationId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public Guid EventId { get; init; }

    /// <inheritdoc />
    public Guid EntityId { get; init; }

    /// <inheritdoc />
    public Guid CorrelationId { get; init; }

    /// <inheritdoc />
    public DateTime CreatedOnUtc { get; init; }
}