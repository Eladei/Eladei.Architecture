using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Запрос для работы с EntityFramework
/// </summary>
/// <typeparam name="R">Тип результата</typeparam>
public interface IDddQuery<R> : IQuery<R>
{
    /// <summary>
    /// Выполнить запрос
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения операции</returns>
    Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}