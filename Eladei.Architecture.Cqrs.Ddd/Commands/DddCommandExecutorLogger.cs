using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Логгер исполнителя команд
/// </summary>
public sealed class DddCommandExecutorLogger : IDddCommandExecutorLogger {
    private readonly ILogger<DddCommandExecutorLogger> _logger;

    public DddCommandExecutorLogger(ILogger<DddCommandExecutorLogger> logger) {
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

    public void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount) {
        var errorMsg = string.Format(
            Resources.CommandUpdateDataSourceAttemptLimitReachedError,
            commandName,
            maxAttemptsCount);

        _logger?.LogError(ex, errorMsg);
    }
}