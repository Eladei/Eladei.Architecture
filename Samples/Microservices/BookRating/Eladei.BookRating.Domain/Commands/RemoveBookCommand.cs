using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Domain.Commands.DomainEvents;
using Eladei.BookRating.Domain.Exceptions;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Commands;

/// <summary>
/// Command for removing a book from the rating
/// </summary>
public sealed class RemoveBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of the RemoveBookCommand class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public RemoveBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.Books.FirstOrDefaultAsync(s => s.Id == _bookId)
            ?? throw new BookWithIdNotFoundException(Resource.BookWithIdNotFound, _bookId);

        context.Books.Remove(book);

        var bookWasRemovedEvent = new BookWasRemovedFromRatingDomainEvent(_bookId);

        SaveDomainEvents(bookWasRemovedEvent);
    }
}