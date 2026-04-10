using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;

namespace CqrsWithDddExecuting.Application;

/// <summary>
/// Команда регистрации книги
/// </summary>
internal sealed class RegisterBookCommand : DddCommandWithResultBase<Guid>
{
    private readonly string _name;
    private readonly string _author;

    /// <summary>
    /// Создает объект класса RegisterBookCommand
    /// </summary>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор книги</param>
    public RegisterBookCommand(string name, string author)
    {
        _name = name;
        _author = author;
    }

    public override async Task<Guid> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var bookId = Guid.NewGuid();
        var book = new BookInRating(bookId, _name, _author);

        await bookRepository.SaveBookAsync(book, cancellationToken);

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingDomainEvent(
            book.Id, book.Name, book.Author);

        AddDomainEvents(bookWasRegisteredEvent);

        return book.Id;
    }
}