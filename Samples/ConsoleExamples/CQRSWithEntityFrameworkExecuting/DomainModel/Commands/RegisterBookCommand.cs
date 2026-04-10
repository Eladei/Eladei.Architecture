using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.DomainModel.Commands;

/// <summary>
/// Команда регистрации книги
/// </summary>
internal sealed class RegisterBookCommand : EfCommandWithResultBase<BookRatingDbContext, Guid>
{
    private readonly string _name;
    private readonly string _author;

    /// <summary>
    /// Создает объект класса RegisterBookCommand
    /// </summary>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор книги</param>
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
            throw new DomainLogicException("Книга уже зарегистрирована");
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
