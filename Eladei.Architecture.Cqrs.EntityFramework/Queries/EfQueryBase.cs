using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Запрос для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public abstract class EfQueryBase<T, R> : IEfQuery<T, R> where T : DbContext{
    public abstract Task<R> ExecuteAsync(T context, CancellationToken cancellationToken = default);
}