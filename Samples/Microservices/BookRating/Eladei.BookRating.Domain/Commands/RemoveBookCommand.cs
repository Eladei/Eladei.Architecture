using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Application.Properties;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Application.Commands;

public sealed class RemoveBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

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

        var bookWasRemovedEvent = new BookWasRemovedFromRatingIntegrationEvent(_bookId);

        AddIntegrationEvents(bookWasRemovedEvent);
    }
}
