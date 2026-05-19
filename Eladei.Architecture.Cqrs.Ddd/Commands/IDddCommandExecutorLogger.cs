using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command executor logger
/// </summary>
public interface IDddCommandExecutorLogger
{
    /// <summary>
    /// Logs the start of command execution
    /// </summary>
    void ExecutingStarted(string commandName);

    /// <summary>
    /// Logs successful completion of command execution
    /// </summary>
    void ExecutingSuccessfulFinished(string commandName);

    /// <summary>
    /// Logs command execution cancellation
    /// </summary>
    /// <param name="ex">The cancellation exception</param>
    void ExecutingCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Logs a domain logic error
    /// </summary>
    /// <param name="ex">The domain logic exception</param>
    void DomainLogicError(string commandName, DomainLogicException ex);

    /// <summary>
    /// Logs a critical command execution error
    /// </summary>
    /// <param name="ex">The execution exception</param>
    void CriticalError(string commandName, Exception ex);

    /// <summary>
    /// Logs an error when retry attempt limit is reached
    /// during database update while executing a command
    /// </summary>
    /// <param name="ex">The execution exception</param>
    /// <param name="maxAttemptsCount">The total number of retry attempts</param>
    void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount);
}