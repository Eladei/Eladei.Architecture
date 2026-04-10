using Eladei.Architecture.Cqrs.Ddd.Properties;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Логгер исполнителя запросов
/// </summary>
public sealed class DddQueryExecutorLogger : IDddQueryExecutorLogger
{
    private readonly ILogger<DddQueryExecutorLogger> _logger;

    public DddQueryExecutorLogger(ILogger<DddQueryExecutorLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ExecutingStarted(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingStarted, queryName);

        _logger?.LogInformation(msg);
    }

    public void ExecutingSuccessfulFinished(string queryName)
    {
        var msg = string.Format(Resources.QueryExecutingSuccessfullyFinished, queryName);

        _logger?.LogInformation(msg);
    }

    public void ExecutingCancelled(string queryName, OperationCanceledException ex)
    {
        var msg = string.Format(Resources.QueryExecutingCancelled, queryName);

        _logger?.LogInformation(ex, msg);
    }

    public void CriticalError<E>(string queryName, E ex) where E : Exception
    {
        var errorMsg = string.Format(Resources.QueryExecutingError, queryName);

        _logger?.LogCritical(ex, errorMsg);
    }
}