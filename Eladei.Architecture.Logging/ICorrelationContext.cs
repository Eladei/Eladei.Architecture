namespace Eladei.Architecture.Logging;

/// <summary>
/// Correlation context
/// </summary>
/// <remarks>
/// Used to propagate a correlationId across the system for tracing
/// the full execution flow of operations
/// </remarks>
public interface ICorrelationContext
{
    /// <summary>
    /// The correlation identifier used for end-to-end tracing
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Sets the correlation identifier for the current execution scope
    /// </summary>
    /// <param name="correlationId">The correlation identifier</param>
    /// <returns>A disposable scope that restores the previous correlation context</returns>
    IDisposable SetCorrelationId(Guid correlationId);
}