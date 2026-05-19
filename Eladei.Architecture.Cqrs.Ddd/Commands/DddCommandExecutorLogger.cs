using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command executor logger
/// </summary>
public sealed class DddCommandExecutorLogger : IDddCommandExecutorLogger
{
    private readonly ILogger<DddCommandExecutorLogger> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="DddCommandExecutorLogger"/>
    /// </summary>
    /// <param name="logger">The logger</param>
    public DddCommandExecutorLogger(ILogger<DddCommandExecutorLogger> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void ExecutingStarted(string commandName)
    {
        var msg = string.Format(Resources.CommandExecutingStarted, commandName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutingSuccessfulFinished(string commandName)
    {
        var msg = string.Format(Resources.CommandExecutingSuccessfullyFinished, commandName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutingCancelled(string commandName, OperationCanceledException ex)
    {
        var msg = string.Format(Resources.CommandExecutingCancelled, commandName);

        _logger?.LogInformation(ex, msg);
    }

    /// <inheritdoc />
    public void DomainLogicError(string commandName, DomainLogicException ex)
    {
        CriticalError(commandName, ex);
    }

    /// <inheritdoc />
    public void CriticalError(string commandName, Exception ex)
    {
        var errorMsg = string.Format(Resources.CommandExecutingError, commandName);

        _logger?.LogCritical(ex, errorMsg);
    }

    /// <inheritdoc />
    public void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount)
    {
        var errorMsg = string.Format(
            Resources.CommandUpdateDataSourceAttemptLimitReachedError,
            commandName,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }
}