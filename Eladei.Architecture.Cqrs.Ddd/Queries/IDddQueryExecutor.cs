namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query executor
/// </summary>
public interface IDddQueryExecutor
{
    /// <summary>
    /// Executes a query
    /// </summary>
    /// <typeparam name="R">The result type</typeparam>
    /// <param name="query">The query</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    Task<R> ExecuteAsync<R>(IDddQuery<R> query, CancellationToken cancellationToken);
}