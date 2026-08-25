using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Application.Commands;

internal sealed class RegisterBookCommand : EfCommandWithResultBase<BookRatingDbContext, Guid>
{
    private readonly string _name;
    private readonly string _author;

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
            throw new EfCommandLogicException("Book is already registered");
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

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingIntegrationEvent(
            book.Id, book.Name, book.Author);

        AddIntegrationEvents(bookWasRegisteredEvent);

        return Task.FromResult(book.Id);
    }
}
