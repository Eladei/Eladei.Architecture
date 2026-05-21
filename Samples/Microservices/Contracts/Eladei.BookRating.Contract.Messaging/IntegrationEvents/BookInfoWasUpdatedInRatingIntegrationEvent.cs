using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// Information about a book was updated in the rating
/// </summary>
public sealed class BookInfoWasUpdatedInRatingIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Creates an instance of the BookInfoWasUpdatedInRatingIntegrationEvent class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="correlationId">Id for distributed tracing</param>
    /// <param name="name">Book title</param>
    /// <param name="author">Author</param>
    public BookInfoWasUpdatedInRatingIntegrationEvent(Guid bookId, Guid correlationId, string name, string author) : base(bookId, correlationId)
    {
        Name = name;
        Author = author;
    }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId { get => EntityId; }

    /// <summary>
    /// Book title
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; }
}