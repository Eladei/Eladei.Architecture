using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// DDD command
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public abstract class DddCommandWithResultBase<R> : IDddCommand<R>
{
    private readonly List<IDomainEvent> _events = [];

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    /// <inheritdoc />
    public void ClearEvents()
    {
        _events.Clear();
    }

    /// <inheritdoc />
    public virtual Task BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public abstract Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds domain events
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <remarks>
    /// Added domain events are available via the <see cref="Events"/> collection.
    /// They are used to allow saving events to the outbox by the command handler
    /// </remarks>
    protected void AddDomainEvents(params IDomainEvent[] domainEvents)
    {
        _events.AddRange(domainEvents);
    }
}