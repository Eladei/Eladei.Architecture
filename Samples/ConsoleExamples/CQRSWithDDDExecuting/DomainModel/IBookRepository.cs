using Eladei.Architecture.Ddd.Repositories;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// Book repository
/// </summary>
public interface IBookRepository : IRepository
{
    /// <summary>
    /// Save book
    /// </summary>
    /// <param name="book">Book</param>
    /// <param name="cancellationToken">Operation cancellation token</param>
    Task SaveBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Update book
    /// </summary>
    /// <param name="book">Book</param>
    /// <param name="cancellationToken">Operation cancellation token</param>
    Task UpdateBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Remove book
    /// </summary>
    /// <param name="book">Book</param>
    /// <param name="cancellationToken">Operation cancellation token</param>
    Task RemoveBookAsync(BookInRating book, CancellationToken cancellationToken);

    /// <summary>
    /// Find book by its identifier
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    /// <param name="cancellationToken">Operation cancellation token</param>
    /// <returns>Search result</returns>
    Task<BookInRating?> FindByIdAsync(Guid bookId, CancellationToken cancellationToken);
}