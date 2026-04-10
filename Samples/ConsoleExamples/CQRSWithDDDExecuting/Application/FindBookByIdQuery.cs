using CqrsWithDddExecuting.DomainModel;
using CqrsWithDddExecuting.ReadModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Queries;
using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.Application;

/// <summary>
/// Запрос информации о книге по ее идентификатору
/// </summary>
internal sealed class FindBookByIdQuery : DddQueryBase<BookInRatingReadModel>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Создает объект класса FindBookByIdQuery
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    public FindBookByIdQuery(Guid bookId)
    {
        _bookId = bookId;
    }

    public override async Task<BookInRatingReadModel> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var bookRepository = repositoryFactory.CreateRepository<IBookRepository>();

        var foundBook = await bookRepository.FindByIdAsync(_bookId, cancellationToken)
            ?? throw new DomainLogicException($"Не найдена книга с указанным Id='{_bookId}'");

        return new BookInRatingReadModel
        {
            BookId = foundBook.Id,
            Name = foundBook.Name,
            Author = foundBook.Author,
            Votes = foundBook.Votes
        };
    }
}