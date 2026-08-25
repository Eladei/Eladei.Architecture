using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Cqrs.Ddd.Queries.Exceptions;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query executor
/// </summary>
public class DddQueryExecutor : IDddQueryExecutor
{
    /// <summary>
    /// The unit of work context factory
    /// </summary>
    protected readonly IUnitOfWorkContextFactory UnitOfWorkContextFactory;

    /// <summary>
    /// The unit of work context factory
    /// </summary>
    protected readonly IDddQueryExecutorLogger? Logger;

    /// <summary>
    /// Creates a new instance of <see cref="DddQueryExecutor"/>
    /// </summary>
    /// <param name="unitOfWorkContextFactory">The unit of work context factory</param>
    /// <param name="logger">The logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DddQueryExecutor(
        IUnitOfWorkContextFactory unitOfWorkContextFactory,
        IDddQueryExecutorLogger? logger = null)
    {
        UnitOfWorkContextFactory = unitOfWorkContextFactory
            ?? throw new ArgumentNullException(nameof(unitOfWorkContextFactory));

        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task<R> ExecuteAsync<R>(IDddQuery<R> query, CancellationToken cancellationToken)
    {
        var queryName = query.GetType().Name;

        Logger?.ExecutingStarted(queryName);

        var unitOfWork = UnitOfWorkContextFactory.CreateContext();

        try
        {
            var result = await query.ExecuteAsync(unitOfWork, cancellationToken);

            Logger?.ExecutingSuccessfulFinished(queryName);

            return result;
        }
        catch (DddQueryLogicException ex)
        {
            Logger?.QueryLogicError(queryName, ex);

            throw;
        }
        catch (OperationCanceledException ex)
        {
            Logger?.ExecutingCancelled(queryName, ex);

            throw;
        }
        catch (Exception ex)
        {
            var unknownEx = new DddQueryExecutingErrorException(Resources.QueryExecutingError, ex);

            Logger?.CriticalError(queryName, unknownEx);

            throw unknownEx;
        }
    }
}
