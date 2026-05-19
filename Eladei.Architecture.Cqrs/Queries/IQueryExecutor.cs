namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Query executor
/// </summary>
public interface IQueryExecutor
{
    /// <summary>
    /// Executes a query
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="query">The query</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The query result</returns>
    Task<T> ExecuteAsync<T>(IQuery<T> query, CancellationToken ct);
}