using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Command for registering a book
/// </summary>
internal sealed class RegisterBookCommand : EfCommandWithResultBase<BookRatingDbContext, Guid>
{
    private readonly string _name;
    private readonly string _author;

    /// <summary>
    /// Creates an instance of RegisterBookCommand
    /// </summary>
    /// <param name="name">Book name</param>
    /// <param name="author">Book author</param>
    /// <exception cref="ArgumentException"></exception>
    public RegisterBookCommand(string name, string author)
    {
        _name = name;
        _author = author;
    }

    public override async Task BeforeExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken = default)
    {
        var isBookRegistered = await context.Books.AnyAsync(
            b => b.Name == _name
                && b.Author == _author,
            cancellationToken);

        if (isBookRegistered)
            throw new DomainLogicException("Book is already registered");
    }

    public override Task<Guid> ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken = default)
    {
        var book = new BookInRatingDb
        {
            Id = Guid.NewGuid(),
            Name = _name,
            Author = _author
        };

        context.Books.Add(book);

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingDomainEvent(
            book.Id, book.Name, book.Author);

        SaveDomainEvents(bookWasRegisteredEvent);

        return Task.FromResult(book.Id);
    }
}