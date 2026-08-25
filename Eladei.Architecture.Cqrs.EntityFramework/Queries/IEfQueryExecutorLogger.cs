using Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Query executor logger
/// </summary>
public interface IEfQueryExecutorLogger
{
    /// <summary>
    /// Logs the start of query execution
    /// </summary>
    /// <param name="queryName">The query name</param>
    void ExecutionStarted(string queryName);

    /// <summary>
    /// Logs successful completion of query execution
    /// </summary>
    /// <param name="queryName">The query name</param>
    void ExecutionSucceeded(string queryName);

    /// <summary>
    /// Logs query execution cancellation
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="ex">The cancellation exception</param>
    void ExecutionCancelled(string queryName, OperationCanceledException ex);

    /// <summary>
    /// Logs query logic error
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="ex">The query logic exception</param>
    void QueryLogicError(string queryName, EfQueryLogicException ex);

    /// <summary>
    /// Logs a critical query execution error
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="ex">The query execution error</param>
    void CriticalError<E>(string queryName, E ex) where E : Exception;
}
