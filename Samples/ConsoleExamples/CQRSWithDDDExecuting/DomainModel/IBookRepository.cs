using Eladei.Architecture.Ddd.Repositories;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// Репозиторий книг
/// </summary>
public interface IBookRepository : IRepository {
    /// <summary>
    /// Сохранить книгу
    /// </summary>
    /// <param name="book">Книга</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить книгу
    /// </summary>
    /// <param name="book">Книга</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить книгу
    /// </summary>
    /// <param name="book">Книга</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task RemoveBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Найти книгу по ее идентификатору
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат поиска</returns>
    Task<BookInRating?> FindByIdAsync(Guid bookId, CancellationToken cancellationToken);
}