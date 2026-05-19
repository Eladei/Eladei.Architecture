namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Command execution error
/// </summary>
public class CommandExecutingErrorException : Exception
{
    public CommandExecutingErrorException() : base() { }

    public CommandExecutingErrorException(string message) : base(message) { }

    public CommandExecutingErrorException(string message, Exception innerException)
        : base(message, innerException) { }
}