using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Entity Framework command executor logger
/// </summary>
public interface IEfCommandExecutorLogger
{
    /// <summary>
    /// Logs the start of command execution
    /// </summary>
    void ExecutionStarted(string commandName);

    /// <summary>
    /// Logs successful completion of command execution
    /// </summary>
    void ExecutionSucceeded(string commandName);

    /// <summary>
    /// Logs command cancellation
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The cancellation exception</param>
    void ExecutionCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Logs a command logic error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The command logic exception</param>
    void CommandLogicError(string commandName, EfCommandLogicException ex);

    /// <summary>
    /// Logs a critical command execution error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The exception</param>
    void CriticalError(string commandName, Exception ex);

    /// <summary>
    /// Logs a database update conflict error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The concurrency exception</param>
    /// <param name="attempt">The current retry attempt</param>
    /// <param name="maxAttemptsCount">The maximum number of retry attempts</param>
    void UpdateError(string commandName, Exception ex, uint attempt, uint maxAttemptsCount);

    /// <summary>
    /// Logs reaching the retry limit for database updates during command execution
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The exception</param>
    /// <param name="maxAttemptsCount">The maximum number of retry attempts</param>
    void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount);
}
