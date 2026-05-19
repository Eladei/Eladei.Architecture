namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Query execution error
/// </summary>
public class QueryExecutingErrorException : Exception
{
    public QueryExecutingErrorException() : base() { }

    public QueryExecutingErrorException(string message) : base(message) { }

    public QueryExecutingErrorException(string message, Exception innerException)
        : base(message, innerException) { }
}