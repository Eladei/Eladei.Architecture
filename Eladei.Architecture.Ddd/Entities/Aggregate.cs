using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Агрегат
/// </summary>
/// <typeparam name="T">Тип идентификатора агрегата</typeparam>
public abstract class Aggregate<T> : IAggregate<T>
{
    private readonly List<IDomainEvent> _domainEvents;

    /// <summary>
    /// Создает объект класса Aggregate
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    public Aggregate(T id)
    {
        Id = id;
        _domainEvents = [];
    }

    /// <summary>
    /// Идентификатор агрегата
    /// </summary>
    public T Id { get; }

    /// <summary>
    /// Доменные события
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Добавить доменное событие
    /// </summary>
    /// <param name="domainEvent">Доменное событие</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (_domainEvents.Any(x => x.EventId == domainEvent.EventId))
            throw new DomainLogicException(
                $"DomainEvent already added to aggregate '{nameof(Aggregate<T>)}' with Id='{Id}'");

        _domainEvents.Add(domainEvent);
    }
}