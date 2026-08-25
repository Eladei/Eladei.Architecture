using Eladei.Architecture.Messaging.IntegrationEvents;

namespace CqrsWithDddExecuting.Application.IntegrationEvents;

public class BookWasRegisteredInRatingIntegrationEvent : IIntegrationEvent
{
    public BookWasRegisteredInRatingIntegrationEvent(Guid bookId, string name, string author) : base()
    {
        BookId = bookId;
        Name = name;
        Author = author;
    }

    public Guid BookId { get; }

    public string Name { get; }

    public string Author { get; }
}
