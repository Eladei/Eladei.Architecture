using Eladei.Architecture.Messaging.IntegrationEvents;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

public class BookWasRegisteredInRatingIntegrationEvent : IIntegrationEvent
{
    public BookWasRegisteredInRatingIntegrationEvent(Guid bookId, string name, string author)
    {
        BookId = bookId;
        Name = name;
        Author = author;
    }

    public Guid BookId { get; }

    public string Name { get; }

    public string Author { get; }
}
