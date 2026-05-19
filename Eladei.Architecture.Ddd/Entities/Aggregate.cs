using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Aggregate root base class
/// </summary>
/// <typeparam name="T">The type of the aggregate identifier</typeparam>
public abstract class Aggregate<T> : IAggregate<T>
{
    private readonly List<IDomainEvent> _domainEvents;

    /// <summary>
    /// Creates an instance of the aggregate
    /// </summary>
    /// <param name="id">The entity identifier</param>
    public Aggregate(T id)
    {
        Id = id;
        _domainEvents = [];
    }

    /// <inheritdoc />
    public T Id { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (_domainEvents.Any(x => x.EventId == domainEvent.EventId))
            throw new DomainLogicException(
                $"DomainEvent already added to aggregate '{nameof(Aggregate<T>)}' with Id='{Id}'");

        _domainEvents.Add(domainEvent);
    }
}