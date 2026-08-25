using Eladei.Architecture.Cqrs.Ddd.Commands.Exceptions;

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
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The cancellation exception</param>
    void ExecutingCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Logs a command logic error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The command logic exception</param>
    void CommandLogicError(string commandName, DddCommandLogicException ex);

    /// <summary>
    /// Logs a critical command execution error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The execution exception</param>
    void CriticalError(string commandName, Exception ex);

    /// <summary>
    /// Logs an error when retry attempt limit is reached
    /// during database update while executing a command
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The execution exception</param>
    /// <param name="maxAttemptsCount">The total number of retry attempts</param>
    void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount);
}
