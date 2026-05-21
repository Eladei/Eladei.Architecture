using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookInfo.Domain.Properties;
using Eladei.BookInfo.Model;
using Eladei.BookInfo.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Domain.Commands;

/// <summary>
/// Command for adding a book
/// </summary>
public sealed class AddBookCommand : EfCommandWithResultBase<BookInfoDbContext, Guid>
{
    private readonly Guid _bookId;
    private readonly string _name;
    private readonly string _author;

    /// <summary>
    /// Creates an instance of AddBookCommand
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="name">Book title</param>
    /// <param name="author">Book author</param>
    /// <exception cref="ArgumentException"></exception>
    public AddBookCommand(Guid bookId, string name, string author)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException(nameof(name));

        if (string.IsNullOrEmpty(author))
            throw new ArgumentException(nameof(author));

        _bookId = bookId;
        _name = name;
        _author = author;
    }

    /// <returns>Identifier of the added book</returns>
    /// <exception cref="BookWithCurrentIdAlreadyExistsException">
    /// Thrown when a book with the specified Id already exists
    /// </exception>
    public override async Task<Guid> ExecuteAsync(
        BookInfoDbContext context,
        CancellationToken cancellationToken)
    {
        var bookExists = await context.BookInformations
            .AnyAsync(s => s.Id == _bookId, cancellationToken);

        if (bookExists)
            throw new BookWithCurrentIdAlreadyExistsException(
                Resources.BookWithCurrentIdAlreadyExists, _bookId);

        var newBook = new BookInformation
        {
            Id = _bookId,
            Name = _name,
            Author = _author
        };

        await context.BookInformations.AddAsync(newBook, cancellationToken);

        return newBook.Id;
    }
}