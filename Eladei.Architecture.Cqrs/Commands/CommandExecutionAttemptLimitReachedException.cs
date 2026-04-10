namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Достигнут лимит попыток выполнения команды
/// </summary>
public class CommandExecutionAttemptLimitReachedException : Exception
{
    public CommandExecutionAttemptLimitReachedException() : base() { }

    public CommandExecutionAttemptLimitReachedException(string message) : base(message) { }

    public CommandExecutionAttemptLimitReachedException(string message, Exception innerException)
        : base(message, innerException) { }
}