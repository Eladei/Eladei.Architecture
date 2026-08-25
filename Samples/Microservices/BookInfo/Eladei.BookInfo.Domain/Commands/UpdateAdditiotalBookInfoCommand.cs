using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookInfo.Application.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Application.Commands;

public sealed class UpdateAdditiotalBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;
    private readonly AdditionalBookInfo _additionalInfo;

    /// <exception cref="ArgumentException"></exception>
    public UpdateAdditiotalBookInfoCommand(Guid bookId, AdditionalBookInfo additionalInfo)
    {
        _bookId = bookId;
        _additionalInfo = additionalInfo
            ?? throw new ArgumentNullException(nameof(additionalInfo));
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(
        BookInfoDbContext context,
        CancellationToken cancellationToken)
    {
        var book = await context.BookInformations
            .FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(
                Resources.BookWithCurrentIdNotExists,
                _bookId);

        book.Pages = _additionalInfo.Pages;
        book.Circulation = _additionalInfo.Circulation;
        book.Annotation = _additionalInfo.Annotation;
        book.Editor = _additionalInfo.Editor;
        book.Translator = _additionalInfo.Translator;
        book.Artist = _additionalInfo.Artist;
    }
}

public sealed record AdditionalBookInfo
{
    public uint? Pages { get; init; }

    public uint? Circulation { get; init; }

    public string? Annotation { get; init; }

    public string? Editor { get; init; }

    public string? Translator { get; init; }

    public string? Artist { get; init; }
}
