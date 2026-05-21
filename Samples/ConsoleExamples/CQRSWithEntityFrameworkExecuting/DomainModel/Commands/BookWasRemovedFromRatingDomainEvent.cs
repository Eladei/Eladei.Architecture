using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Domain event for removing a book from the rating
/// </summary>
public class BookWasRemovedFromRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Creates an instance of BookWasRemovedFromRatingDomainEvent
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public BookWasRemovedFromRatingDomainEvent(Guid bookId) : base(bookId) { }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId => EntityId;
}