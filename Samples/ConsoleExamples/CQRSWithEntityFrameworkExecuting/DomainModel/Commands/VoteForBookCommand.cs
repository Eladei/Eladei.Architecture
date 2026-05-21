using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Command for voting for a book
/// </summary>
internal sealed class VoteForBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of VoteForBookCommand
    /// </summary>
    /// <param name="bookId">Book identifier</param>
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
            ?? throw new DomainLogicException("Book is not registered");

        book.Votes++;

        SaveDomainEvents(new BookWasVotedDomainEvent(_bookId));
    }
}