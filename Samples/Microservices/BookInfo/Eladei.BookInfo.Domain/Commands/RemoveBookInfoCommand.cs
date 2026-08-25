using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookInfo.Application.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Application.Commands;

public sealed class RemoveBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;

    public RemoveBookInfoCommand(Guid bookId)
    {
        _bookId = bookId;
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

        context.BookInformations.Remove(book);
    }
}
