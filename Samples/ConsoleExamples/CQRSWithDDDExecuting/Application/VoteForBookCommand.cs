using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.Application;

/// <summary>
/// Command for voting for a book
/// </summary>
internal sealed class VoteForBookCommand : DddCommandBase
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of VoteForBookCommand
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DomainLogicException($"Book with specified Id='{_bookId}' was not found");

        foundBook.Vote();

        await bookRepository.UpdateBookAsync(foundBook, cancellationToken);

        AddDomainEvents([.. foundBook.DomainEvents]);
    }
}