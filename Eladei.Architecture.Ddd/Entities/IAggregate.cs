using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Агрегат
/// </summary>
public interface IAggregate
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Добавить событие предметной области
    /// </summary>
    /// <param name="domainEvent"></param>
    void AddDomainEvent(IDomainEvent domainEvent);
}

/// <summary>
/// Агрегат
/// </summary>
public interface IAggregate<T> : IAggregate, IEntity<T> { }