using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.BookRating.Domain.Commands.DomainEvents;

/// <summary>
/// Информация о книге была обновлена в рейтинге
/// </summary>
public sealed class BookInfoWasUpdatedInRatingDomainEvent : DomainEvent {
    /// <summary>
    /// Создает объект класса BookInfoWasUpdatedInRatingDomainEvent 
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор</param>
    public BookInfoWasUpdatedInRatingDomainEvent(Guid bookId, string name, string author) : base(bookId) {
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