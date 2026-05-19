using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Query executor for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
public class EfQueryExecutor<T> : IEfQueryExecutor<T> where T : DbContext
{
    protected readonly IDbContextFactory<T> _contextFactory;
    protected readonly IEfQueryExecutorLogger? _logger;

    /// <summary>
    /// Creates an instance of the EF query executor
    /// </summary>
    /// <param name="contextFactory">The database context factory</param>
    /// <param name="logger">The optional logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfQueryExecutor(IDbContextFactory<T> contextFactory, IEfQueryExecutorLogger? logger = null)
    {
        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        _logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken)
    {
        var queryName = query.GetType().Name;

        _logger?.ExecutingStarted(queryName);

        using var dbContext = await CreateDbContextAsync(queryName, cancellationToken);

        try
        {
            var result = await query.ExecuteAsync(dbContext, cancellationToken);

            _logger?.ExecutingSuccessfulFinished(queryName);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger?.ExecutingCancelled(queryName, ex);

            throw;
        }
        catch (Exception ex)
        {
            var unknownEx = new QueryExecutingErrorException(Resources.QueryExecutingError, ex);

            _logger?.CriticalError(queryName, unknownEx);

            throw unknownEx;
        }
    }

    /// <summary>
    /// Creates a database context
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The database context instance</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual async Task<T> CreateDbContextAsync(string queryName, CancellationToken cancellationToken)
    {
        T context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            _logger?.CriticalError(queryName, invalidOperEx);

            throw invalidOperEx;
        }

        return context;
    }
}