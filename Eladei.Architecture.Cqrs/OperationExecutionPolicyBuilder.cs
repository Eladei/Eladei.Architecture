using Eladei.Architecture.Cqrs.Properties;
using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Builder for creating operation execution policies
/// </summary>
/// <remarks>
/// By default, retry attempts are not allowed
/// for domain logic failures
/// </remarks>
public class OperationExecutionPolicyBuilder
{
    private IReadOnlyCollection<Type>? _exceptionTypesForRetry;
    private uint _maxAttemptsCount;
    private uint _maxDelayInMilliseconds;

    /// <summary>
    /// Creates a new instance of <see cref="OperationExecutionPolicyBuilder"/>
    /// </summary>
    /// <remarks>
    /// By default, retry attempts are not allowed
    /// for domain logic failures
    /// </remarks>
    public OperationExecutionPolicyBuilder()
    {
        _maxAttemptsCount = 1;
        _maxDelayInMilliseconds = 5000;
    }

    /// <summary>
    /// Configures retry attempts for the specified exception types
    /// </summary>
    /// <param name="exceptionTypesForRetry">Exception types that allow retry attempts</param>
    /// <returns>The current builder instance</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    /// By default, retry attempts are not allowed
    /// for domain logic failures
    /// </remarks>
    public virtual OperationExecutionPolicyBuilder RetryOn(params Type[] exceptionTypesForRetry)
    {
        if (exceptionTypesForRetry.Any(e => typeof(DomainLogicException).IsAssignableFrom(e)))
        {
            var error = string.Format(Resources.UnsupportedExceptionType, nameof(DomainLogicException));

            throw new ArgumentException(error, nameof(DomainLogicException));
        }

        _exceptionTypesForRetry = exceptionTypesForRetry.AsReadOnly();

        return this;
    }

    /// <summary>
    /// Configures the maximum number of execution attempts
    /// </summary>
    /// <param name="maxAttemptsCount">
    /// Maximum number of execution attempts
    /// </param>
    /// <returns>The current builder instance</returns>
    public virtual OperationExecutionPolicyBuilder MaxAttemptsCount(uint maxAttemptsCount)
    {
        if (maxAttemptsCount == 0)
            throw new InvalidOperationException(Resources.MaxAttemptsCountMustBeGreaterThanZero);

        _maxAttemptsCount = maxAttemptsCount;

        return this;
    }

    /// <summary>
    /// Configures the maximum delay before the next retry attempt
    /// </summary>
    /// <param name="maxDelayInMilliseconds">
    /// Maximum delay, in milliseconds, before the next retry attempt
    /// </param>
    /// <returns>The current builder instance</returns>
    public virtual OperationExecutionPolicyBuilder MaxDelayInMilliseconds(uint maxDelayInMilliseconds)
    {
        _maxDelayInMilliseconds = maxDelayInMilliseconds;

        return this;
    }

    /// <summary>
    /// Builds the operation execution policy based on the configured parameters
    /// </summary>
    /// <returns></returns>
    public IOperationExecutionPolicy Build()
    {
        return new OperationExecutionPolicy()
        {
            ExceptionTypesForRetry = _exceptionTypesForRetry,
            MaxAttemptsCount = _maxAttemptsCount,
            MaxDelayInMilliseconds = _maxDelayInMilliseconds,
        };
    }

    /// <summary>
    /// Operation execution policy
    /// </summary>
    internal sealed class OperationExecutionPolicy : IOperationExecutionPolicy
    {
        internal IReadOnlyCollection<Type>? ExceptionTypesForRetry { get; init; }

        /// <summary>
        /// Maximum number of execution attempts
        /// </summary>
        public uint MaxAttemptsCount { get; init; }

        /// <summary>
        /// Maximum delay before the next retry attempt
        /// </summary>
        public uint MaxDelayInMilliseconds { get; init; }

        /// <summary>
        /// Determines whether the operation should be retried
        /// </summary>
        /// <typeparam name="T">The type of the thrown exception</typeparam>
        /// <param name="ex">The exception that caused the operation failure</param>
        /// <param name="currentAttempt">The current attempt number</param>
        /// <returns>
        /// <see langword="true"/> if the operation should be retried;
        /// otherwise, <see langword="false"/>
        /// </returns>
        public bool ShouldRetry<T>(T ex, uint currentAttempt) where T : Exception
        {
            if (currentAttempt >= MaxAttemptsCount)
                return false;

            return ExceptionTypesForRetry is null
                || ExceptionTypesForRetry.Any(x => x.IsAssignableFrom(ex.GetType()));
        }
    }
}