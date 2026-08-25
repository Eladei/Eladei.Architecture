using CqrsWithDddExecuting.DomainModel;
using CqrsWithDddExecuting.ReadModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Queries;
using Eladei.Architecture.Cqrs.Ddd.Queries.Exceptions;

namespace CqrsWithDddExecuting.Application;

internal sealed class FindBookByIdQuery : DddQueryBase<BookInRatingReadModel>
{
    private readonly Guid _bookId;

    public FindBookByIdQuery(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task<BookInRatingReadModel> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DddQueryLogicException($"Book with specified Id='{_bookId}' was not found");

        return new BookInRatingReadModel
        {
            BookId = foundBook.Id,
            Name = foundBook.Name,
            Author = foundBook.Author,
            Votes = foundBook.Votes
        };
    }
}
