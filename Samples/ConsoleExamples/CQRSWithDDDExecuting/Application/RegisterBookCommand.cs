using CqrsWithDddExecuting.Application.IntegrationEvents;
using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;

namespace CqrsWithDddExecuting.Application;

internal sealed class RegisterBookCommand : DddCommandWithResultBase<Guid>
{
    private readonly string _name;
    private readonly string _author;

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

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingIntegrationEvent(
            book.Id, book.Name, book.Author);

        AddIntegrationEvents(bookWasRegisteredEvent);

        return book.Id;
    }
}
