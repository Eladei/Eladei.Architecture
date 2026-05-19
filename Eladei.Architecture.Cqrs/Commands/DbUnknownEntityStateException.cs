namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// The database entity is in an unknown state
/// </summary>
public class DbUnknownEntityStateException : Exception
{
    public DbUnknownEntityStateException() : base() { }

    public DbUnknownEntityStateException(string message) : base(message) { }

    public DbUnknownEntityStateException(string message, Exception innerException)
        : base(message, innerException) { }
}