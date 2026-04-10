namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Политика выполнения операции
/// </summary>
public interface IOperationExecutionPolicy
{
    /// <summary>
    /// Допустимое число попыток выполнения операции
    /// </summary>
    uint MaxAttemptsCount { get; }

    /// <summary>
    /// Максимальная задержка перед следующей попыткой выполнения операции
    /// </summary>
    uint MaxDelayInMilliseconds { get; }

    /// <summary>
    /// Возможность повторного выполнения команды
    /// </summary>
    /// <typeparam name="T">Тип исключения</typeparam>
    /// <param name="ex">Исключение</param>
    /// <param name="currentAttempt">Текущущая попытка</param>
    /// <returns>Возможность повторного выполнения команды</returns>
    bool ShouldRetry<T>(T ex, uint currentAttempt) where T : Exception;
}