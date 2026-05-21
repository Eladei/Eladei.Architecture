using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Queries;

/// <summary>
/// Query for finding a book by its identifier
/// </summary>
internal sealed class FindBookByIdQuery : EfQueryBase<BookRatingDbContext, BookInRatingReadModel>
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

    public override async Task<BookInRatingReadModel> ExecuteAsync(
        BookRatingDbContext context,
        CancellationToken cancellationToken = default)
    {
        var book = await context.Books
            .FirstOrDefaultAsync(b => b.Id == _bookId, cancellationToken)
            ?? throw new DomainLogicException("Book is not registered");

        return new BookInRatingReadModel
        {
            BookId = book.Id,
            Name = book.Name,
            Author = book.Author,
            Votes = book.Votes
        };
    }
}