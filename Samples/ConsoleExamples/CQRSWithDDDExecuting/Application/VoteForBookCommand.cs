using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.Application;

/// <summary>
/// Команда регистрации книги
/// </summary>
internal sealed class VoteForBookCommand : DddCommandBase
{
    private readonly Guid _bookId;

    /// <summary>
    /// Создает объект класса RegisterBookCommand
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DomainLogicException($"Не найдена книга с указанным Id='{_bookId}'");

        foundBook.Vote();

        await bookRepository.UpdateBookAsync(foundBook, cancellationToken);

        AddDomainEvents([.. foundBook.DomainEvents]);
    }
}