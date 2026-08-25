namespace Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

/// <summary>
/// The database entity is in an unknown state
/// </summary>
public class DbUnknownEntityStateException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbUnknownEntityStateException"/> class
    /// </summary>
    public DbUnknownEntityStateException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbUnknownEntityStateException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DbUnknownEntityStateException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbUnknownEntityStateException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DbUnknownEntityStateException(string message, Exception innerException)
        : base(message, innerException) { }
}
