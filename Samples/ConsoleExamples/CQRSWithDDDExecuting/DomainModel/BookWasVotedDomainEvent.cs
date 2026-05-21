using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// Domain event for voting for a book in the rating
/// </summary>
public class BookWasVotedDomainEvent : DomainEvent
{
    /// <summary>
    /// Creates an instance of BookWasVotedDomainEvent
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public BookWasVotedDomainEvent(Guid bookId) : base(bookId) { }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId => EntityId;
}