namespace Eladei.Architecture.Cqrs.Ddd.Queries.Exceptions;

/// <summary>
/// Query execution logic error
/// </summary>
public class DddQueryLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryLogicException"/> class
    /// </summary>
    public DddQueryLogicException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DddQueryLogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DddQueryLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}
