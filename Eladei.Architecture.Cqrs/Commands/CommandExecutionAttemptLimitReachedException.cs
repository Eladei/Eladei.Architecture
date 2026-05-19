namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Execution attempt limit has been reached
/// </summary>
public class CommandExecutionAttemptLimitReachedException : Exception
{
    public CommandExecutionAttemptLimitReachedException() : base() { }

    public CommandExecutionAttemptLimitReachedException(string message) : base(message) { }

    public CommandExecutionAttemptLimitReachedException(string message, Exception innerException)
        : base(message, innerException) { }
}