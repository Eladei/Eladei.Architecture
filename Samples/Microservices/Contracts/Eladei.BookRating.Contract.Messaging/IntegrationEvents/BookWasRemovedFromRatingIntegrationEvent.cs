using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// Книга была удалена из рейтинга
/// </summary>
public class BookWasRemovedFromRatingIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Создает объект класса BookWasRemovedFromRatingIntegrationEvent
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    public BookWasRemovedFromRatingIntegrationEvent(Guid bookId, Guid correlationId) : base(bookId, correlationId) { }

    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId { get => EntityId; }
}