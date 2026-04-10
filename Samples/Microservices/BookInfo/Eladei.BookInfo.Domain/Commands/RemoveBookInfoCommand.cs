using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookInfo.Domain.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Domain.Commands;

/// <summary>
/// Команда обновления информации о книге
/// </summary>
public sealed class RemoveBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Создает объект класса RemoveBookInfoCommand
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public RemoveBookInfoCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookInfoDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.BookInformations
            .FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(Resources.BookWithCurrentIdNotExists, _bookId);

        context.BookInformations.Remove(book);
    }
}