namespace Eladei.Architecture.Ddd.DomainEvents;

/// <summary>
/// Domain event
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Event identifier
    /// </summary>
    Guid EventId { get; }
}