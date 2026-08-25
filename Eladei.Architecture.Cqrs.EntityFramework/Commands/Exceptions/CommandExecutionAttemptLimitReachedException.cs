namespace Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

/// <summary>
/// Execution attempt limit has been reached
/// </summary>
public class CommandExecutionAttemptLimitReachedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="CommandExecutionAttemptLimitReachedException"/> class
    /// </summary>
    public CommandExecutionAttemptLimitReachedException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="CommandExecutionAttemptLimitReachedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public CommandExecutionAttemptLimitReachedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="CommandExecutionAttemptLimitReachedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public CommandExecutionAttemptLimitReachedException(string message, Exception innerException)
        : base(message, innerException) { }
}
