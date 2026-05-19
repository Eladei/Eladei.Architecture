using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Base query for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <typeparam name="R">The result type</typeparam>
public abstract class EfQueryBase<T, R> : IEfQuery<T, R> where T : DbContext
{
    /// <inheritdoc />
    public abstract Task<R> ExecuteAsync(T context, CancellationToken cancellationToken = default);
}