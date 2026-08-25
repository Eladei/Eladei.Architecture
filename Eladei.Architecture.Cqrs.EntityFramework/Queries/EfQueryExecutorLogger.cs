using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Entity Framework query executor logger
/// </summary>
public sealed class EfQueryExecutorLogger : IEfQueryExecutorLogger
{
    private readonly ILogger<EfQueryExecutorLogger> _logger;

    /// <inheritdoc />
    public EfQueryExecutorLogger(ILogger<EfQueryExecutorLogger> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void ExecutionStarted(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingStarted, queryName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutionSucceeded(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingSuccessfullyFinished, queryName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutionCancelled(string queryName, OperationCanceledException ex)
    {
        var msg = string.Format(Resources.QueryExecutingCancelled, queryName);

        _logger?.LogInformation(ex, msg);
    }

    /// <inheritdoc />
    public void QueryLogicError(string queryName, EfQueryLogicException ex)
    {
        CriticalError(queryName, ex);
    }

    /// <inheritdoc />
    public void CriticalError<E>(string queryName, E ex) where E : Exception
    {
        var errorMsg = string.Format(Resources.QueryExecutingError, queryName);

        _logger?.LogCritical(ex, errorMsg);
    }
}
