using CqrsWithDddExecuting.DomainModel;
using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Ddd.Repositories;

namespace CqrsWithDddExecuting.Infrastructure; 

/// <summary>
/// Мок контекста единицы работы рейтинга книг
/// </summary>
public sealed class MockBookRatingUnitOfWorkContext : IUnitOfWorkContext {
    private List<BookInRatingDb> _dataContext;

    /// <summary>
    /// Создает объект класса MockBookRatingUnitOfWorkContext
    /// </summary>
    /// <param name="dataContext">Контекст данных</param>
    public MockBookRatingUnitOfWorkContext(List<BookInRatingDb> dataContext) {
        _dataContext = dataContext 
            ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task CommitTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task RollbackTransactionAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public R CreateRepository<R>() where R : IRepository {
        if (typeof(R) == typeof(IBookRepository))
            return (R)(IBookRepository)new MockBookRepository(_dataContext);

        throw new NotImplementedException();
    }
}