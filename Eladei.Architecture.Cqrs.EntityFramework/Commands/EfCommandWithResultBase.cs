using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Command that returns a result
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <typeparam name="R">The result type</typeparam>
/// <remarks>
/// The command directly works with the database context and follows the transaction script pattern
/// </remarks>
public abstract class EfCommandWithResultBase<T, R> : IEfCommand<T, R> where T : DbContext
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
    public virtual Task BeforeExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public abstract Task<R> ExecuteAsync(T context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds integration events
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <remarks>
    /// The added integration events are available through the Events collection.
    /// They are used to persist events in the outbox by the command executor
    /// </remarks>
    protected void AddIntegrationEvents(params IIntegrationEvent[] integrationEvents)
    {
        _events.AddRange(integrationEvents);
    }
}
