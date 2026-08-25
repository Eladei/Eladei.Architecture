using Eladei.Architecture.Messaging.IntegrationEvents;

namespace CqrsWithDddExecuting.Application.IntegrationEvents;

public class BookWasVotedIntegrationEvent : IIntegrationEvent
{
    public BookWasVotedIntegrationEvent(Guid bookId) : base()
    {
        BookId = bookId;
    }

    public Guid BookId { get; }
}
