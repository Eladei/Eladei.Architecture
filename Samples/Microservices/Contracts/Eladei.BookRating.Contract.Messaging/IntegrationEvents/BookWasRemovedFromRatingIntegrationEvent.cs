using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// A book was removed from the rating
/// </summary>
public class BookWasRemovedFromRatingIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Creates an instance of the BookWasRemovedFromRatingIntegrationEvent class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public BookWasRemovedFromRatingIntegrationEvent(Guid bookId)
    {
        BookId = bookId;
    }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId { get; }
}
