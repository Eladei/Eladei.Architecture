using Eladei.Architecture.Cqrs.Properties;
using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Строитель для формирования политик выполнения операций
/// </summary>
/// <remarks>По умолчанию не допускает повторные попытки 
/// выполнения операции при ошибке доменной логики</remarks>
public class OperationExecutionPolicyBuilder
{
    private IReadOnlyCollection<Type>? _exceptionTypesForRetry;
    private uint _maxAttemptsCount;
    private uint _maxDelayInMilliseconds;

    /// <summary>
    /// Создает объект класса OperationExecutionPolicyBuilder
    /// </summary>
    /// <remarks>По умолчанию не допускает повторные попытки выполнения операции при ошибке доменной логики</remarks>
    public OperationExecutionPolicyBuilder()
    {
        _maxAttemptsCount = 1;
        _maxDelayInMilliseconds = 5000;
    }

    /// <summary>
    /// Установить количество повторных попыток выполнения операции
    /// </summary>
    /// <param name="exceptionTypesForRetry">Количество повторных попыток выполнения</param>
    /// <returns>Политика выполнения операции</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>По умолчанию метод не допускает повторные попытки выполнения операции при ошибке доменной логики</remarks>
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

    public virtual OperationExecutionPolicyBuilder MaxAttemptsCount(uint maxAttemptsCount)
    {
        if (maxAttemptsCount == 0)
            throw new InvalidOperationException(Resources.MaxAttemptsCountMustBeGreaterThanZero);

        _maxAttemptsCount = maxAttemptsCount;

        return this;
    }

    public virtual OperationExecutionPolicyBuilder MaxDelayInMilliseconds(uint maxDelayInMilliseconds)
    {
        _maxDelayInMilliseconds = maxDelayInMilliseconds;

        return this;
    }

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
    /// Политика выполнения операции
    /// </summary>
    internal sealed class OperationExecutionPolicy : IOperationExecutionPolicy
    {
        internal IReadOnlyCollection<Type>? ExceptionTypesForRetry { get; init; }

        /// <summary>
        /// Допустимое число попыток выполнения операции
        /// </summary>
        public uint MaxAttemptsCount { get; init; }

        /// <summary>
        /// Максимальная задержка перед следующей попыткой выполнения операции
        /// </summary>
        public uint MaxDelayInMilliseconds { get; init; }

        public bool ShouldRetry<T>(T ex, uint currentAttempt) where T : Exception
        {
            if (currentAttempt >= MaxAttemptsCount)
                return false;

            return ExceptionTypesForRetry is null
                || ExceptionTypesForRetry.Any(x => x.IsAssignableFrom(ex.GetType()));
        }
    }
}