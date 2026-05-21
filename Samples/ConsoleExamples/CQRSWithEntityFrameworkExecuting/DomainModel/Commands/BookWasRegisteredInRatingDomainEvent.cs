using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Domain event for registering a book in the rating
/// </summary>
public class BookWasRegisteredInRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Creates an instance of BookWasRegisteredInRatingDomainEvent
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="name">Book name</param>
    /// <param name="author">Author</param>
    public BookWasRegisteredInRatingDomainEvent(Guid bookId, string name, string author) : base(bookId)
    {
        Name = name;
        Author = author;
    }

    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId => EntityId;

    /// <summary>
    /// Book name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; }
}