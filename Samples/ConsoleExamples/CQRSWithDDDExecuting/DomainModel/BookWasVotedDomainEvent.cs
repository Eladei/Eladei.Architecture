using Eladei.Architecture.Ddd.DomainEvents;

namespace CqrsWithDddExecuting.DomainModel;

public class BookWasVotedDomainEvent : DomainEvent
{
    public BookWasVotedDomainEvent(Guid bookId) : base()
    {
        BookId = bookId;
    }

    public Guid BookId { get; }
}
