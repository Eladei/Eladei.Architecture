using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.BookRating.Domain.Commands.DomainEvents;

/// <summary>
/// Книга была зарегистрирована в рейтинге
/// </summary>
public sealed class BookWasRegisteredInRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Создает объект класса BookWasRegisteredInRatingDomainEvent
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор</param>
    public BookWasRegisteredInRatingDomainEvent(Guid bookId, string name, string author) : base(bookId)
    {
        Name = name;
        Author = author;
    }

    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId => EntityId;

    /// <summary>
    /// Название книги
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; }
}