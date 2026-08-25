namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Exception thrown when a domain rule is violated
/// </summary>
public class DomainLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DomainLogicException"/> class
    /// </summary>
    public DomainLogicException() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DomainLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DomainLogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DomainLogicException"/> class
    /// </summary>
    /// <param name="format">The error message format</param>
    /// <param name="arguments">Args of the error message</param>
    public DomainLogicException(string format, params object?[] arguments)
        : base(string.Format(format, arguments)) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DomainLogicException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DomainLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}
