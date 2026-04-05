using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// Информация о книге была обновлена в рейтинге
/// </summary>
public sealed class BookInfoWasUpdatedInRatingIntegrationEvent : IntegrationEvent {
    /// <summary>
    /// Создает объект класса BookInfoWasUpdatedInRatingIntegrationEvent  
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор</param>
    public BookInfoWasUpdatedInRatingIntegrationEvent(Guid bookId, Guid correlationId, string name, string author) : base(bookId, correlationId) {
        Name = name;
        Author = author;
    }

    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId { get => EntityId; }

    /// <summary>
    /// Название книги
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; }
}