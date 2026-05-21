using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.BookRating.Domain.Commands.DomainEvents;

/// <summary>
/// A book was registered in the rating
/// </summary>
public sealed class BookWasRegisteredInRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Creates an instance of the BookWasRegisteredInRatingDomainEvent class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="name">Book title</param>
    /// <param name="author">Author</param>
    public BookWasRegisteredInRatingDomainEvent(Guid bookId, string name, string author)
        : base(bookId)
    {
        Name = name;
        Author = author;
    }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId => EntityId;

    /// <summary>
    /// Book title
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; }
}