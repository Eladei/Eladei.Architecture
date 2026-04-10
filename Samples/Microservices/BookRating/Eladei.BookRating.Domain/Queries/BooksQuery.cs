using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.BookRating.Domain.Queries.ReadModel;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Queries;

/// <summary>
/// Запрос для получения списка книг
/// </summary>
public sealed class BooksQuery : EfPageQueryBase<BookRatingDbContext, BookReadModel>
{
    /// <summary>
    /// Создает объект класса BooksQuery
    /// </summary>
    /// <param name="booksPerPage">Количество книг на странице</param>
    /// <param name="page">Номер целевой страницы</param>>
    public BooksQuery(uint booksPerPage, uint page) : base(booksPerPage, page) { }

    /// <summary>
    /// Запросить список книг
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список книг</returns>
    protected override async Task<IEnumerable<BookReadModel>> PerformAsync(BookRatingDbContext context, CancellationToken cancellationToken)
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

    protected override async Task<uint> GetAllElementsCount(BookRatingDbContext context, CancellationToken cancellationToken)
        => (uint)await context.Books.CountAsync(cancellationToken);
}