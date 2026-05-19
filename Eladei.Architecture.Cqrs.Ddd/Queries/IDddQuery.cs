using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public interface IDddQuery<R> : IQuery<R>
{
    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The execution result</returns>
    Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}