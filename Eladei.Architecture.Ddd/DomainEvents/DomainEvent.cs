namespace Eladei.Architecture.Ddd.DomainEvents;

/// <summary>
/// Domain event base class
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Creates an instance of the domain event
    /// </summary>
    /// <param name="entityId">The identifier of the entity associated with the event</param>
    public DomainEvent(Guid entityId)
    {
        EventId = Guid.NewGuid();
        EntityId = entityId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public Guid EventId { get; init; }

    /// <summary>
    /// The identifier of the entity associated with the event
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// The UTC timestamp when the event was created
    /// </summary>
    public DateTime CreatedOnUtc { get; init; }
}