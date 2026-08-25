namespace Eladei.Architecture.Cqrs.Ddd.Commands.Exceptions;

/// <summary>
/// Command execution error
/// </summary>
public class DddCommandLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddCommandLogicException"/> class
    /// </summary>
    public DddCommandLogicException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddCommandLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DddCommandLogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddCommandLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DddCommandLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}
