using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookInfo.Domain.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Domain.Commands;

/// <summary>
/// Команда обновления дополнительной информации о книге
/// </summary>
public sealed class UpdateAdditiotalBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;
    private readonly AdditionalBookInfo _additionalInfo;

    /// <summary>
    /// Создает объект класса UpdateAdditiotalBookInfoCommand
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="additionalInfo">Дополнительная информация о книге</param>
    /// <exception cref="ArgumentException"></exception>
    public UpdateAdditiotalBookInfoCommand(Guid bookId, AdditionalBookInfo additionalInfo)
    {
        _bookId = bookId;
        _additionalInfo = additionalInfo
            ?? throw new ArgumentNullException(nameof(additionalInfo));
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookInfoDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.BookInformations
            .FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(Resources.BookWithCurrentIdNotExists, _bookId);

        book.Pages = _additionalInfo.Pages;
        book.Circulation = _additionalInfo.Circulation;
        book.Annotation = _additionalInfo.Annotation;
        book.Editor = _additionalInfo.Editor;
        book.Translator = _additionalInfo.Translator;
        book.Artist = _additionalInfo.Artist;
    }
}

/// <summary>
/// Дополнительная информация о книге
/// </summary>
public record AdditionalBookInfo
{
    /// <summary>
    /// Число страниц
    /// </summary>
    public uint? Pages { get; init; }

    /// <summary>
    /// Тираж
    /// </summary>
    public uint? Circulation { get; init; }

    /// <summary>
    /// Аннотация
    /// </summary>
    public string? Annotation { get; init; }

    /// <summary>
    /// Редактор
    /// </summary>
    public string? Editor { get; init; }

    /// <summary>
    /// Переводчик
    /// </summary>
    public string? Translator { get; init; }

    /// <summary>
    /// Художник
    /// </summary>
    public string? Artist { get; init; }
}