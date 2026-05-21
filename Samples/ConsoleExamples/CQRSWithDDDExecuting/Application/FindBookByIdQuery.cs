using CqrsWithDddExecuting.DomainModel;
using CqrsWithDddExecuting.ReadModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Queries;
using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.Application;

/// <summary>
/// Query for getting book information by its identifier
/// </summary>
internal sealed class FindBookByIdQuery : DddQueryBase<BookInRatingReadModel>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of FindBookByIdQuery
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public FindBookByIdQuery(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task<BookInRatingReadModel> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DomainLogicException($"Book with specified Id='{_bookId}' was not found");

        return new BookInRatingReadModel
        {
            BookId = foundBook.Id,
            Name = foundBook.Name,
            Author = foundBook.Author,
            Votes = foundBook.Votes
        };
    }
}