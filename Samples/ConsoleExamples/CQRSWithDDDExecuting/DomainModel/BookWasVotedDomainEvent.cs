using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// События голосования за книгу в рейтинге
/// </summary>
public class BookWasVotedDomainEvent : DomainEvent {
    /// <summary>
    /// Создает объект класса BookWasVotedDomainEvent
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public BookWasVotedDomainEvent(Guid bookId) : base(bookId) { }

    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId => EntityId;
}