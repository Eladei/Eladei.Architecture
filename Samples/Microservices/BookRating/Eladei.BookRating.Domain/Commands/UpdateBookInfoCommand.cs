using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Domain.Commands.DomainEvents;
using Eladei.BookRating.Domain.Exceptions;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Commands;

/// <summary>
/// Command for updating book information in the rating
/// </summary>
public sealed class UpdateBookInfoCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;
    private readonly string _newName;
    private readonly string _newAuthor;

    /// <summary>
    /// Creates an instance of the UpdateBookInfoCommand class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="newName">New title</param>
    /// <param name="newAuthor">New author</param>
    /// <exception cref="ArgumentException"></exception>
    public UpdateBookInfoCommand(Guid bookId, string newName, string newAuthor)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException(nameof(bookId));

        if (string.IsNullOrEmpty(newName))
            throw new ArgumentException(nameof(newName));

        if (string.IsNullOrEmpty(newAuthor))
            throw new ArgumentException(nameof(newAuthor));

        _bookId = bookId;
        _newName = newName;
        _newAuthor = newAuthor;
    }

    /// <exception cref="BookWithCurrentInfoAlreadyExistsException"></exception>
    public override async Task<bool> BeforeExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var bookWithNewInfoExists = await context.Books.AnyAsync(
            s => s.Name == _newName
                && s.Author == _newAuthor
                && s.Id != _bookId,
            cancellationToken);

        if (bookWithNewInfoExists)
            throw new BookWithCurrentInfoAlreadyExistsException(
                Resource.BookWithCurrentInfoAlreadyExists, _newName, _newAuthor);

        return true;
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.Books.FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(Resource.BookWithIdNotFound, _bookId);

        book.Name = _newName;
        book.Author = _newAuthor;

        var bookInfoWasUpdatedEvent = new BookInfoWasUpdatedInRatingDomainEvent(
            book.Id, book.Name, book.Author);

        SaveDomainEvents(bookInfoWasUpdatedEvent);

        context.Books.Update(book);
    }
}