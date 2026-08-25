namespace Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

/// <summary>
/// The entity being deleted has already been removed from the database
/// </summary>
public class DbRemovingObjectWasRemovedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbRemovingObjectWasRemovedException"/> class
    /// </summary>
    public DbRemovingObjectWasRemovedException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbRemovingObjectWasRemovedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DbRemovingObjectWasRemovedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbRemovingObjectWasRemovedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DbRemovingObjectWasRemovedException(string message, Exception innerException)
        : base(message, innerException) { }
}
