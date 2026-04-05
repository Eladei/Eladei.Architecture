namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Исполнитель запроса
/// </summary>
public interface IQueryExecutor {
    /// <summary>
    /// Выполнить запрос
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого результата</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат запроса</returns>
    Task<T> ExecuteAsync<T>(IQuery<T> query, CancellationToken ct);
}