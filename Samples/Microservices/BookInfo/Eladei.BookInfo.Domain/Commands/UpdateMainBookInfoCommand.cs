using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookInfo.Application.Properties;
using Eladei.BookInfo.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Application.Commands;

public sealed class UpdateMainBookInfoCommand : EfCommandBase<BookInfoDbContext>
{
    private readonly Guid _bookId;
    private readonly string _name;
    private readonly string _author;

    /// <exception cref="ArgumentException"></exception>
    public UpdateMainBookInfoCommand(Guid bookId, string name, string author)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException(nameof(name));

        if (string.IsNullOrEmpty(author))
            throw new ArgumentException(nameof(author));

        _bookId = bookId;
        _name = name;
        _author = author;
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookInfoDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.BookInformations
            .FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(Resources.BookWithCurrentIdNotExists, _bookId);

        book.Name = _name;
        book.Author = _author;
    }
}
