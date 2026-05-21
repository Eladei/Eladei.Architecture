using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookInfo.Domain.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Domain.Commands;

/// <summary>
/// Command for updating additional book information
/// </summary>
public sealed class UpdateAdditiotalBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;
    private readonly AdditionalBookInfo _additionalInfo;

    /// <summary>
    /// Creates an instance of UpdateAdditiotalBookInfoCommand
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="additionalInfo">Additional book information</param>
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

/// <summary>
/// Additional book information
/// </summary>
public record AdditionalBookInfo
{
    /// <summary>
    /// Number of pages
    /// </summary>
    public uint? Pages { get; init; }

    /// <summary>
    /// Print run (circulation)
    /// </summary>
    public uint? Circulation { get; init; }

    /// <summary>
    /// Annotation / summary
    /// </summary>
    public string? Annotation { get; init; }

    /// <summary>
    /// Editor
    /// </summary>
    public string? Editor { get; init; }

    /// <summary>
    /// Translator
    /// </summary>
    public string? Translator { get; init; }

    /// <summary>
    /// Illustrator / artist
    /// </summary>
    public string? Artist { get; init; }
}