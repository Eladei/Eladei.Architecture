using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookInfo.Application.Properties;
using Eladei.BookInfo.Application.Queries.ReadModel;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Application.Queries;

public sealed class BookInfoQuery : EfQueryBase<BookInfoDbContext, BookInfoReadModel>
{
    private readonly Guid _bookId;

    public BookInfoQuery(Guid bookId)
    {
        _bookId = bookId;
    }

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
