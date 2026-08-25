namespace Eladei.Architecture.Ddd.DomainEvents;

/// <summary>
/// Domain event base class
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Creates an instance of the domain event
    /// </summary>
    public DomainEvent()
    {
        EventId = Guid.NewGuid();
    }

    /// <inheritdoc />
    public Guid EventId { get; init; }
}
