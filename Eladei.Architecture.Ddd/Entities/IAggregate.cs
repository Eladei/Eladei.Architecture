using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Aggregate root
/// </summary>
public interface IAggregate
{
    /// <summary>
    /// Domain events raised by the aggregate
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to the aggregate
    /// </summary>
    /// <param name="domainEvent">The domain event</param>
    void AddDomainEvent(IDomainEvent domainEvent);
}

/// <summary>
/// Aggregate root with identifier
/// </summary>
/// <typeparam name="T">The type of the entity identifier</typeparam>
public interface IAggregate<T> : IAggregate, IEntity<T> { }