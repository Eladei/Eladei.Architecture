namespace Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

/// <summary>
/// Command execution error
/// </summary>
public class EfCommandLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfCommandLogicException"/> class
    /// </summary>
    public EfCommandLogicException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfCommandLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public EfCommandLogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfCommandLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public EfCommandLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}
