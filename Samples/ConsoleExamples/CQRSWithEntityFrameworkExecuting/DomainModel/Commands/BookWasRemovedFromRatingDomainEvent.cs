using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Событие удаления книги из рейтинга
/// </summary>
public class BookWasRemovedFromRatingDomainEvent : DomainEvent
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