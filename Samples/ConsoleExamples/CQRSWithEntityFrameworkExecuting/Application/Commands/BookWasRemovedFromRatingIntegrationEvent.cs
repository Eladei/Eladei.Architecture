using Eladei.Architecture.Messaging.IntegrationEvents;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

public class BookWasRemovedFromRatingIntegrationEvent : IIntegrationEvent
{
    public BookWasRemovedFromRatingIntegrationEvent(Guid bookId)
    {
        BookId = bookId;
    }

    public Guid BookId { get; }
}
 