using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Base command for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <remarks>
/// Command directly works with the database context and implements the transaction script pattern
/// </remarks>
public abstract class EfCommandBase<T> : IEfCommand<T> where T : DbContext
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
    public virtual Task<bool> BeforeExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public abstract Task ExecuteAsync(T context, CancellationToken cancellationToken = default);

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
