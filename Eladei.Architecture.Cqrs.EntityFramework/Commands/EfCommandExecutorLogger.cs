using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Логгер исполнителя команд
/// </summary>
public sealed class EfCommandExecutorLogger : IEfCommandExecutorLogger {
    private readonly ILogger<EfCommandExecutorLogger> _logger;

    public EfCommandExecutorLogger(ILogger<EfCommandExecutorLogger> logger) {
        _logger = logger;
    }

    public void ExecutingStarted(string commandName) {
        var msg = string.Format(Resources.CommandExecutingStarted, commandName);

        _logger?.LogInformation(msg);
    }

    public void ExecutingSuccessfulFinished(string commandName) {
        var msg = string.Format(Resources.CommandExecutingSuccessfullyFinished, commandName);

        _logger?.LogInformation(msg);
    }

    public void ExecutingCancelled(string commandName, OperationCanceledException ex) {
        var msg = string.Format(Resources.CommandExecutingCancelled, commandName);

        _logger?.LogInformation(ex, msg);
    }

    public void DomainLogicError(string commandName, DomainLogicException ex) {
        CriticalError(commandName, ex);
    }

    public void CriticalError(string commandName, Exception ex) {
        var errorMsg = string.Format(Resources.CommandExecutingError, commandName);

        _logger?.LogCritical(ex, errorMsg);
    }

    public void UpdateError(string commandName, Exception ex, uint attempt, uint maxAttemptsCount) {
        var errorMsg = string.Format(
            Resources.CommandDbUpdateAttemptError,
            commandName,
            attempt,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }

    public void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount) {
        var errorMsg = string.Format(
            Resources.CommandDbUpdateAttemptLimitReachedError,
            commandName,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }
}