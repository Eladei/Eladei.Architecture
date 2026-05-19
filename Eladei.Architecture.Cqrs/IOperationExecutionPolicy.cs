namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Defines a policy for operation execution and retry behavior.
/// </summary>
/// <remarks>
/// Implementations control retry attempts, retry delays,
/// and determine whether an operation should be retried
/// for a given exception.
/// </remarks>
public interface IOperationExecutionPolicy
{
    /// <summary>
    /// Maximum number of execution attempts,
    /// including the initial attempt.
    /// </summary>
    uint MaxAttemptsCount { get; }

    /// <summary>
    /// Maximum delay, in milliseconds,
    /// before the next retry attempt.
    /// </summary>
    /// <remarks>
    /// Implementations may apply strategies such as
    /// exponential backoff or jitter, but should not
    /// exceed this limit.
    /// </remarks>
    uint MaxDelayInMilliseconds { get; }

    /// <summary>
    /// Determines whether the operation should be retried
    /// for the specified exception and attempt number.
    /// </summary>
    /// <typeparam name="T">The type of the thrown exception.</typeparam>
    /// <param name="ex">The exception that caused the operation failure.</param>
    /// <param name="currentAttempt">The current attempt number (1-based).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the operation should be retried;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool ShouldRetry<T>(T ex, uint currentAttempt) where T : Exception;
}