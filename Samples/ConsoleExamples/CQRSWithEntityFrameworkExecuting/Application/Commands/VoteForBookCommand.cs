using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

internal sealed class VoteForBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(
        BookRatingDbContext context,
        CancellationToken cancellationToken = default)
    {
        var book = await context.Books.FirstOrDefaultAsync(
            b => b.Id == _bookId, cancellationToken)
            ?? throw new EfCommandLogicException("Book is not registered");

        book.Votes++;

        AddIntegrationEvents(new BookWasVotedIntegrationEvent(_bookId));
    }
}
