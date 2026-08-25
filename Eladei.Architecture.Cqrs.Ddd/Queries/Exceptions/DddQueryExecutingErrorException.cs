namespace Eladei.Architecture.Cqrs.Ddd.Queries.Exceptions;

/// <summary>
/// Query execution error
/// </summary>
public class DddQueryExecutingErrorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryExecutingErrorException"/> class
    /// </summary>
    public DddQueryExecutingErrorException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryExecutingErrorException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DddQueryExecutingErrorException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <seealso cref="DddQueryExecutingErrorException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public DddQueryExecutingErrorException(string message, Exception innerException)
        : base(message, innerException) { }
}
