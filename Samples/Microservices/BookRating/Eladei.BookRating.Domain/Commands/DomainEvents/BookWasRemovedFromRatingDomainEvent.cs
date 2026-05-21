using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.BookRating.Domain.Commands.DomainEvents;

/// <summary>
/// A book was removed from the rating
/// </summary>
public sealed class BookWasRemovedFromRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Creates an instance of the BookWasRemovedFromRatingDomainEvent class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public BookWasRemovedFromRatingDomainEvent(Guid bookId) : base(bookId) { }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId => EntityId;
}