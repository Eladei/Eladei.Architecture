namespace Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

/// <summary>
/// The entity being modified has already been deleted from the database
/// </summary>
public class DbModifiedObjectWasRemovedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbModifiedObjectWasRemovedException"/> class
    /// </summary>
    public DbModifiedObjectWasRemovedException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbModifiedObjectWasRemovedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DbModifiedObjectWasRemovedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DbModifiedObjectWasRemovedException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DbModifiedObjectWasRemovedException(string message, Exception innerException)
        : base(message, innerException) { }
}
