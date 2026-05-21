using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// A book was removed from the rating
/// </summary>
public class BookWasRemovedFromRatingIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Creates an instance of the BookWasRemovedFromRatingIntegrationEvent class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="correlationId">Id for distributed tracing</param>
    public BookWasRemovedFromRatingIntegrationEvent(Guid bookId, Guid correlationId) : base(bookId, correlationId) { }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId { get => EntityId; }
}