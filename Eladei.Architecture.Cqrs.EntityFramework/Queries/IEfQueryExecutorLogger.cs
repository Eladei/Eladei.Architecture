namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Логгер исполнителя запросов
/// </summary>
public interface IEfQueryExecutorLogger
{
    /// <summary>
    /// Логировать начало обработки запроса
    /// </summary>
    /// <param name="queryName">Название запроса</param>
    void ExecutingStarted(string queryName);

    /// <summary>
    /// Логировать успешное завершение обработки запроса
    /// </summary>
    /// <param name="queryName">Название запроса</param>
    void ExecutingSuccessfulFinished(string queryName);

    /// <summary>
    /// Логировать завершение обработки запроса
    /// </summary>
    /// <param name="queryName">Название запроса</param>
    /// <param name="ex">Данные по отмене операции</param>
    void ExecutingCancelled(string queryName, OperationCanceledException ex);

    /// <summary>
    /// Логировать критическую ошибку обработки запроса
    /// </summary>
    /// <param name="queryName">Название запроса</param>
    /// <param name="ex">Ошибка обработки команды</param>
    void CriticalError<E>(string queryName, E ex) where E : Exception;
}