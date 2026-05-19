using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Query for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <typeparam name="R">The result type</typeparam>
public interface IEfQuery<T, R> : IQuery<R>
{
    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The operation result</returns>
    Task<R> ExecuteAsync(T context, CancellationToken cancellationToken);
}