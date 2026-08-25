using CqrsWithDddExecuting.Application.IntegrationEvents;
using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Cqrs.Ddd.Commands.Exceptions;

namespace CqrsWithDddExecuting.Application;

internal sealed class VoteForBookCommand : DddCommandBase
{
    private readonly Guid _bookId;

    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DddCommandLogicException($"Book with specified Id='{_bookId}' was not found");

        foundBook.Vote();

        await bookRepository.UpdateBookAsync(foundBook, cancellationToken);

        var expectedEvent = foundBook.DomainEvents.First(x => x is BookWasVotedDomainEvent) as BookWasVotedDomainEvent;
        var integrationEvent = new BookWasVotedIntegrationEvent(expectedEvent!.BookId);

        AddIntegrationEvents(integrationEvent);
    }
}
