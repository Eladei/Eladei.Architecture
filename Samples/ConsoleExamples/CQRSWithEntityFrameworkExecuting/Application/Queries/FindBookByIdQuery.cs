using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Application.Queries;

internal sealed class FindBookByIdQuery : EfQueryBase<BookRatingDbContext, BookInRatingReadModel>
{
    private readonly Guid _bookId;

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
            ?? throw new EfQueryLogicException("Book is not registered");

        return new BookInRatingReadModel
        {
            BookId = book.Id,
            Name = book.Name,
            Author = book.Author,
            Votes = book.Votes
        };
    }
}
