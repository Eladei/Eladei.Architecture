namespace Eladei.Architecture.Ddd.DomainEvents;

/// <summary>
/// Доменное событие
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Id события
    /// </summary>
    Guid EventId { get; }
}