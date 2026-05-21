using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookInfo.Domain.Properties;
using Eladei.BookInfo.Domain.Queries.ReadModel;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Domain.Queries;

/// <summary>
/// Query for retrieving book information
/// </summary>
public sealed class BookInfoQuery : EfQueryBase<BookInfoDbContext, BookInfoReadModel>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of <see cref="BookInfoQuery"/>
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public BookInfoQuery(Guid bookId)
    {
        _bookId = bookId;
    }

    /// <summary>
    /// Retrieves book information
    /// </summary>
    /// <param name="context">Data context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Book information</returns>
    public override async Task<BookInfoReadModel> ExecuteAsync(
        BookInfoDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.BookInformations
            .Select(b => new BookInfoReadModel
            {
                Id = b.Id,
                Name = b.Name,
                Author = b.Author,
                Pages = b.Pages,
                Circulation = b.Circulation,
                Annotation = b.Annotation,
                Editor = b.Editor,
                Translator = b.Translator,
                Artist = b.Artist
            })
            .FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(
                Resources.BookWithCurrentIdNotExists, _bookId);

        return book;
    }
}