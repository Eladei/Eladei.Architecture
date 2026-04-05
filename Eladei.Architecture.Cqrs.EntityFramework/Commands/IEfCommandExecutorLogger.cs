using Eladei.Architecture.Ddd.Entities;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands; 

/// <summary>
/// Логгер исполнителя команд
/// </summary>
public interface IEfCommandExecutorLogger {
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
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Данные по отмене операции</param>
    void ExecutingCancelled(string commandName, OperationCanceledException ex);

    /// <summary>
    /// Логировать ошибку доменной логики
    /// </summary>
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Ошибка доменной логики</param>
    void DomainLogicError(string commandName, DomainLogicException ex);

    /// <summary>
    /// Логировать критическую ошибку обработки команды
    /// </summary>
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Ошибка обработки команды</param>
    void CriticalError(string commandName, Exception ex);

    /// <summary>
    /// Логировать ошибку обновления информации в БД
    /// </summary>
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Ошибка конкурентного доступа</param>
    /// <param name="attempt">Текущая попытка обновления БД</param>
    /// <param name="maxAttemptsCount">Общее количество попыток обновления БД</param>
    void UpdateError(string commandName, Exception ex, uint attempt, uint maxAttemptsCount);

    /// <summary>
    /// Логировать ошибку достижения предела попыток обновления информации в БД
    /// в процессе обработки команды
    /// </summary>
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Ошибка обработки команды</param>
    /// <param name="maxAttemptsCount">Общее количество попыток обновления БД</param>
    void AttemptLimitReachedError(string commandName, Exception ex, uint maxAttemptsCount);
}