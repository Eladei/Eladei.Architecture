namespace Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;

/// <summary>
/// Query execution error
/// </summary>
public class EfQueryExecutingErrorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryExecutingErrorException"/> class
    /// </summary>
    public EfQueryExecutingErrorException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryExecutingErrorException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public EfQueryExecutingErrorException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="EfQueryExecutingErrorException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public EfQueryExecutingErrorException(string message, Exception innerException)
        : base(message, innerException) { }
}
