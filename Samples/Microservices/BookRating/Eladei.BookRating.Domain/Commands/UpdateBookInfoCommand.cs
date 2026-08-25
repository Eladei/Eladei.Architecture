using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Application.Properties;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Application.Commands;

public sealed class UpdateBookInfoCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;
    private readonly string _newName;
    private readonly string _newAuthor;

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

        var bookInfoWasUpdatedEvent = new BookInfoWasUpdatedInRatingIntegrationEvent(
            book.Id, book.Name, book.Author);

        AddIntegrationEvents(bookInfoWasUpdatedEvent);

        context.Books.Update(book);
    }
}
