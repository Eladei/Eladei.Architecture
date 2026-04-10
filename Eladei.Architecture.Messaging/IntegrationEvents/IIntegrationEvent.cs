namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Событие интеграции
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Id события
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Id сущности, к которой относится событие
    /// </summary>
    Guid EntityId { get; }

    /// <summary>
    /// Id для сквозного отслеживания
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Дата создания события в стандарте UTC
    /// </summary>
    DateTime CreatedOnUtc { get; }
}