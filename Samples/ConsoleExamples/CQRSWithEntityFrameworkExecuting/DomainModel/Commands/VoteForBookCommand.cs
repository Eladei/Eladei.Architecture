using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Команда голосования за книгу
/// </summary>
internal sealed class VoteForBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Создает объект класса VoteForBookCommand
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken = default)
    {
        var removingBook = await context.Books.FirstOrDefaultAsync(
            b => b.Id == _bookId, cancellationToken)
            ?? throw new DomainLogicException("Книга не зарегистрирована");

        removingBook.Votes++;

        SaveDomainEvents(new BookWasVotedDomainEvent(_bookId));
    }
}