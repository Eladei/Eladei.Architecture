using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

internal sealed class RemoveBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    public RemoveBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken = default)
    {
        var removingBook = await context.Books.FirstOrDefaultAsync(
            b => b.Id == _bookId, cancellationToken)
            ?? throw new EfCommandLogicException("Book is not registered");

        context.Books.Remove(removingBook);

        AddIntegrationEvents(new BookWasRemovedFromRatingIntegrationEvent(_bookId));
    }
}
