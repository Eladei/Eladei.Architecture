using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Entity Framework command executor logger
/// </summary>
public interface IEfCommandExecutorLogger
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
    /// Logs command cancellation
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The cancellation exception</param>
    void ExecutingCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Logs a domain logic error
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The domain logic exception</param>
    void DomainLogicError(string commandName, DomainLogicException ex);

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