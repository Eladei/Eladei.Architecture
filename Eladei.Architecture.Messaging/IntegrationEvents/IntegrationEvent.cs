namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Событие интеграции
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent {
    /// <summary>
    /// Конструктор класса IntegrationEvent
    /// </summary>
    /// <param name="entityId">Id сущности, к которой относится событие</param>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    public IntegrationEvent(Guid entityId, Guid correlationId) { 
        EventId = Guid.NewGuid();
        EntityId = entityId;
        CorrelationId = correlationId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Id события
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// Id сущности, к которой относится событие
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Id для сквозного отслеживания
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary>
    /// Дата создания события в стандарте UTC
    /// </summary>
    public DateTime CreatedOnUtc { get; init; }
}