namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Исполнитель запросов
/// </summary>
public interface IDddQueryExecutor
{
    /// <summary>
    /// Выполнить запрос
    /// </summary>
    /// <typeparam name="R">Тип возвращаемого результата</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат запроса</returns>
    Task<R> ExecuteAsync<R>(IDddQuery<R> query, CancellationToken cancellationToken);
}