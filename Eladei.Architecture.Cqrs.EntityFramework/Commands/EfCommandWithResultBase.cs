using Eladei.Architecture.Ddd.DomainEvents;
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
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// Domain events
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

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
    /// Adds domain events
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <remarks>
    /// The added domain events are available through the Events collection.
    /// They are used to persist events in the outbox by the command executor
    /// </remarks>
    protected void SaveDomainEvents(params IDomainEvent[] domainEvents)
    {
        _events.AddRange(domainEvents);
    }
}