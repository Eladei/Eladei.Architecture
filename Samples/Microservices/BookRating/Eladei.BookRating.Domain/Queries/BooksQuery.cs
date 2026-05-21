using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.BookRating.Domain.Queries.ReadModel;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Queries;

/// <summary>
/// Query for retrieving a list of books
/// </summary>
public sealed class BooksQuery : EfPageQueryBase<BookRatingDbContext, BookReadModel>
{
    /// <summary>
    /// Creates an instance of the BooksQuery class
    /// </summary>
    /// <param name="booksPerPage">Number of books per page</param>
    /// <param name="page">Target page number</param>
    public BooksQuery(uint booksPerPage, uint page)
        : base(booksPerPage, page) { }

    /// <summary>
    /// Executes query to retrieve books
    /// </summary>
    /// <param name="context">Data context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of books</returns>
    protected override async Task<IEnumerable<BookReadModel>> PerformAsync(
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var query = context.Books
            .OrderByDescending(s => s.Votes)
            .Skip((int)ElementsToSkip);

        if (_elementsPerPage.HasValue)
            query = query.Take((int)_elementsPerPage);

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