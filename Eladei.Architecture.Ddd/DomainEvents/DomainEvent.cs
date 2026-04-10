namespace Eladei.Architecture.Ddd.DomainEvents;

/// <summary>
/// Доменное событие
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Конструктор класса DomainEvent
    /// </summary>
    /// <param name="entityId">Id сущности, к которой относится событие</param>
    public DomainEvent(Guid entityId)
    {
        EventId = Guid.NewGuid();
        EntityId = entityId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Id события
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// Id сущности, к которой относится событие
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// Дата создания события в стандарте UTC
    /// </summary>
    public DateTime CreatedOnUtc { get; init; }
}