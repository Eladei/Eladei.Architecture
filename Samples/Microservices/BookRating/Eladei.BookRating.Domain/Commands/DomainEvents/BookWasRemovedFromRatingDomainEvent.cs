using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.BookRating.Domain.Commands.DomainEvents;

/// <summary>
/// Книга была удалена из рейтинга
/// </summary>
public sealed class BookWasRemovedFromRatingDomainEvent : DomainEvent
{
    /// <summary>
    /// Создает объект класса BookWasRemovedFromRatingDomainEvent
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public BookWasRemovedFromRatingDomainEvent(Guid bookId) : base(bookId) { }

    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId => EntityId;
}