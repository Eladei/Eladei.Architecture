using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Command for removing a book
/// </summary>
internal sealed class RemoveBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of RemoveBookCommand
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public RemoveBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken = default)
    {
        var removingBook = await context.Books.FirstOrDefaultAsync(
            b => b.Id == _bookId, cancellationToken)
            ?? throw new DomainLogicException("Book is not registered");

        context.Books.Remove(removingBook);

        SaveDomainEvents(new BookWasRemovedFromRatingDomainEvent(_bookId));
    }
}