using Eladei.Architecture.Cqrs.EntityFramework.Properties;
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
    public void ExecutingStarted(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingStarted, queryName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutingSuccessfulFinished(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingSuccessfullyFinished, queryName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutingCancelled(string queryName, OperationCanceledException ex)
    {
        var msg = string.Format(Resources.QueryExecutingCancelled, queryName);

        _logger?.LogInformation(ex, msg);
    }

    /// <inheritdoc />
    public void CriticalError<E>(string queryName, E ex) where E : Exception
    {
        var errorMsg = string.Format(Resources.QueryExecutingError, queryName);

        _logger?.LogCritical(ex, errorMsg);
    }
}
