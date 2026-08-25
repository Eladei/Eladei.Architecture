using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.BookRating.Application.Queries.ReadModel;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Application.Queries;

public sealed class BooksQuery : EfPageQueryBase<BookRatingDbContext, BookReadModel>
{
    public BooksQuery(uint booksPerPage, uint page)
        : base(booksPerPage, page) { }

    protected override async Task<IEnumerable<BookReadModel>> PerformAsync(
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var query = context.Books
            .OrderByDescending(s => s.Votes)
            .Skip((int)ElementsToSkip);

        if (ElementsPerPage.HasValue)
            query = query.Take((int)ElementsPerPage);

        var bookInfos = await query.Select(s => new BookReadModel
        {
            Id = s.Id,
            Name = s.Name,
            Author = s.Author,
            Votes = s.Votes,
            RegisteredAtUtc = s.CreatedAtUtc
        }).ToArrayAsync(cancellationToken);

        return bookInfos;
    }

    /// <inheritdoc />
    protected override async Task<uint> GetAllElementsCount(
        BookRatingDbContext context,
        CancellationToken cancellationToken)
        => (uint)await context.Books.CountAsync(cancellationToken);
}
