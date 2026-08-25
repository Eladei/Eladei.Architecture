using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Command executor logger
/// </summary>
public sealed class EfCommandExecutorLogger : IEfCommandExecutorLogger
{
    private readonly ILogger<EfCommandExecutorLogger> _logger;

    /// <summary>
    /// Creates an instance of the command executor logger
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public EfCommandExecutorLogger(ILogger<EfCommandExecutorLogger> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void ExecutionStarted(string commandName)
    {
        var msg = string.Format(Resources.CommandExecutingStarted, commandName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutionSucceeded(string commandName)
    {
        var msg = string.Format(Resources.CommandExecutingSuccessfullyFinished, commandName);

        _logger?.LogInformation(msg);
    }

    /// <inheritdoc />
    public void ExecutionCancelled(string commandName, OperationCanceledException ex)
    {
        var msg = string.Format(Resources.CommandExecutingCancelled, commandName);

        _logger?.LogInformation(ex, msg);
    }

    /// <inheritdoc />
    public void CommandLogicError(string commandName, EfCommandLogicException ex)
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
    public void UpdateError(string commandName, Exception ex, uint attempt, uint maxAttemptsCount)
    {
        var errorMsg = string.Format(
            Resources.CommandDbUpdateAttemptError,
            commandName,
            attempt,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }

    /// <inheritdoc />
    public void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount)
    {
        var errorMsg = string.Format(
            Resources.CommandDbUpdateAttemptLimitReachedError,
            commandName,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }
}
