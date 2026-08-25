using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Query executor for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
public class EfQueryExecutor<T> : IEfQueryExecutor<T> where T : DbContext
{
    /// <summary>
    /// The database context factory
    /// </summary>
    protected readonly IDbContextFactory<T> ContextFactory;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly IEfQueryExecutorLogger? Logger;

    /// <summary>
    /// Creates an instance of the EF query executor
    /// </summary>
    /// <param name="contextFactory">The database context factory</param>
    /// <param name="logger">The logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfQueryExecutor(IDbContextFactory<T> contextFactory, IEfQueryExecutorLogger? logger = null)
    {
        ContextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken)
    {
        var queryName = query.GetType().Name;

        Logger?.ExecutionStarted(queryName);

        using var dbContext = await CreateDbContextAsync(queryName, cancellationToken);

        try
        {
            var result = await query.ExecuteAsync(dbContext, cancellationToken);

            Logger?.ExecutionSucceeded(queryName);

            return result;
        }
        catch (EfQueryLogicException ex)
        {
            Logger?.QueryLogicError(queryName, ex);

            throw;
        }
        catch (OperationCanceledException ex)
        {
            Logger?.ExecutionCancelled(queryName, ex);

            throw;
        }
        catch (Exception ex)
        {
            var unknownEx = new EfQueryExecutingErrorException(Resources.QueryExecutingError, ex);

            Logger?.CriticalError(queryName, unknownEx);

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
        T context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            Logger?.CriticalError(queryName, invalidOperEx);

            throw invalidOperEx;
        }

        return context;
    }
}
