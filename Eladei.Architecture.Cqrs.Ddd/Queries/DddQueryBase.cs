namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query base class
/// </summary>
public abstract class DddQueryBase<R> : IDddQuery<R>
{
    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    public abstract Task<R> ExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default);
}