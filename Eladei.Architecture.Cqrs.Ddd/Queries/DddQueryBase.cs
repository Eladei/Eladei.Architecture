namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Запрос
/// </summary>
public abstract class DddQueryBase<R> : IDddQuery<R>
{
    public abstract Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default);
}