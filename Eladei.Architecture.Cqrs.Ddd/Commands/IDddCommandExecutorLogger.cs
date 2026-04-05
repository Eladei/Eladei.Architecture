using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs.Ddd.Commands; 

/// <summary>
/// Логгер исполнителя команд
/// </summary>
public interface IDddCommandExecutorLogger {
    /// <summary>
    /// Логировать начало обработки команды
    /// </summary>
    void ExecutingStarted(string commandName);

    /// <summary>
    /// Логировать успешное завершение обработки команды
    /// </summary>
    void ExecutingSuccessfulFinished(string commandName);

    /// <summary>
    /// Логировать завершение обработки команды
    /// </summary>
    /// <param name="ex">Данные по отмене операции</param>
    void ExecutingCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Логировать ошибку доменной логики
    /// </summary>
    /// <param name="ex">Ошибка доменной логики</param>
    void DomainLogicError(string commandName, DomainLogicException ex);

    /// <summary>
    /// Логировать критическую ошибку обработки команды
    /// </summary>
    /// <param name="ex">Ошибка обработки команды</param>
    void CriticalError(string commandName, Exception ex);

    /// <summary>
    /// Логировать ошибку достижения предела попыток обновления информации в БД
    /// в процессе обработки команды
    /// </summary>
    /// <param name="ex">Ошибка обработки команды</param>
    /// <param name="maxAttemptsCount">Общее количество попыток обновления БД</param>
    void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount);
}
