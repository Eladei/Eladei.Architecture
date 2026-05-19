namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query executor logger
/// </summary>
public interface IDddQueryExecutorLogger
{
    /// <summary>
    /// Logs the start of query execution
    /// </summary>
    /// <param name="queryName">The query name</param>
    void ExecutingStarted(string queryName);

    /// <summary>
    /// Logs successful completion of query execution
    /// </summary>
    /// <param name="queryName">The query name</param>
    void ExecutingSuccessfulFinished(string queryName);

    /// <summary>
    /// Logs query execution cancellation
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="ex">The cancellation exception</param>
    void ExecutingCancelled(string queryName, OperationCanceledException ex);

    /// <summary>
    /// Logs a critical query execution error
    /// </summary>
    /// <param name="queryName">The query name</param>
    /// <param name="ex">The execution exception</param>
    void CriticalError<E>(string queryName, E ex) where E : Exception;
}