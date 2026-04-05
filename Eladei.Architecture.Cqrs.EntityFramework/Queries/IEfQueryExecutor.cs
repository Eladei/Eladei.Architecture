using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Исполнитель запроса, работающей с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public interface IEfQueryExecutor<T> where T : DbContext {
    /// <summary>
    /// Выполнить запрос
    /// </summary>
    /// <typeparam name="R">Тип возвращаемого результата</typeparam>
    /// <param name="query">Запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат запроса</returns>
    Task<R> ExecuteAsync<R>(IEfQuery<T, R> query, CancellationToken cancellationToken);
}