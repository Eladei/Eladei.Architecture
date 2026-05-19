namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Query base class
/// </summary>
public abstract class DddQueryBase<R> : IDddQuery<R>
{
    /// <inheritdoc />
    public abstract Task<R> ExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default);
}