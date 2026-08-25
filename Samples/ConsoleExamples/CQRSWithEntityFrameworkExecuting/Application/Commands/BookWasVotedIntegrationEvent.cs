using Eladei.Architecture.Messaging.IntegrationEvents;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

public class BookWasVotedIntegrationEvent : IIntegrationEvent
{
    public BookWasVotedIntegrationEvent(Guid bookId)
    {
        BookId = bookId;
    }

    public Guid BookId { get; }
}
