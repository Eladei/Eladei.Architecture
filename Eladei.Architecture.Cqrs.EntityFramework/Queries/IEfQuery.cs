using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Запрос для работы с EntityFramework
/// </summary>
/// <typeparam name="R">Тип результата</typeparam>
public interface IEfQuery<T, R> : IQuery<R> {
    /// <summary>
    /// Выполнить запрос
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения операции</returns>
    Task<R> ExecuteAsync(T context, CancellationToken cancellationToken);
}