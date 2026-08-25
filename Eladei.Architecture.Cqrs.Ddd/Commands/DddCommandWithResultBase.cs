using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// DDD command
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public abstract class DddCommandWithResultBase<R> : IDddCommand<R>
{
    private readonly List<IIntegrationEvent> _events = [];

    /// <inheritdoc />
    public IReadOnlyCollection<IIntegrationEvent> Events => _events.AsReadOnly();

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
    /// Adds integration events
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <remarks>
    /// Added integration events are available via the <see cref="Events"/> collection.
    /// They are used to allow saving events to the outbox by the command handler
    /// </remarks>
    protected void AddIntegrationEvents(params IIntegrationEvent[] integrationEvents)
    {
        _events.AddRange(integrationEvents);
    }
}
