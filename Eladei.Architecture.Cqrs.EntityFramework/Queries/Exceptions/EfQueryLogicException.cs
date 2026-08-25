namespace Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;

/// <summary>
/// Query execution logic error
/// </summary>
public class EfQueryLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryLogicException"/> class
    /// </summary>
    public EfQueryLogicException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public EfQueryLogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public EfQueryLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}
