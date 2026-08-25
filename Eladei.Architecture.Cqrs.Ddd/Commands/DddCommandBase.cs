using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Base class for commands
/// </summary>
public abstract class DddCommandBase : IDddCommand
{
    private readonly List<IIntegrationEvent> _events = [];

    /// <inheritdoc />
    public IReadOnlyCollection<IIntegrationEvent> Events => _events;

    /// <inheritdoc />
    public void ClearEvents()
    {
        _events.Clear();
    }

    /// <inheritdoc />
    public virtual Task<bool> BeforeExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public abstract Task ExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds integration events
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <remarks>
    /// Added integration events are available via the <see cref="Events"/> collection.
    /// They are used to persist events via the command handler outbox mechanism
    /// </remarks>
    protected void AddIntegrationEvents(params IIntegrationEvent[] integrationEvents)
    {
        _events.AddRange(integrationEvents);
    }
}
