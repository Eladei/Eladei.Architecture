using Eladei.Architecture.Ddd.Repositories;

namespace CqrsWithDddExecuting.DomainModel;

public interface IBookRepository : IRepository
{
    Task SaveBookAsync(BookInRating book, CancellationToken cancellationToken);

    Task UpdateBookAsync(BookInRating book, CancellationToken cancellationToken);

    Task RemoveBookAsync(BookInRating book, CancellationToken cancellationToken);

    Task<BookInRating?> FindByIdAsync(Guid bookId, CancellationToken cancellationToken);
}
