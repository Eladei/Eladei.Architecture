using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Contract.Messaging.IntegrationEvents;

/// <summary>
/// Книга была зарегистрирована в рейтинге
/// </summary>
public class BookWasRegisteredInRatingIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Создает объект класса BookWasRegisteredInRatingIntegrationEvent
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор</param>
    public BookWasRegisteredInRatingIntegrationEvent(Guid bookId, Guid correlationId, string name, string author) : base(bookId, correlationId)
    {
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