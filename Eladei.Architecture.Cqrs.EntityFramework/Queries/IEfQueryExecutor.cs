using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Query executor for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
public interface IEfQueryExecutor<T> where T : DbContext
{
    /// <summary>
    /// Executes a query
    /// </summary>
    /// <typeparam name="R">The result type</typeparam>
    /// <param name="query">The query</param>
    /// <param name="cancellationToken">The operation cancellation token</param>
    /// <returns>The query result</returns>
    Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken);
}