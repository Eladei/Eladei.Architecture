namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Unit of work
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Commits the transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves changes
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}