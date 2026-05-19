using Eladei.Architecture.Ddd.DomainEvents;
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
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyCollection<IDomainEvent> Events => _events;

    public void ClearEvents()
    {
        _events.Clear();
    }

    public virtual Task<bool> BeforeExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public abstract Task ExecuteAsync(T context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds domain events
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <remarks>
    /// Added domain events are available via the <see cref="Events"/> collection.
    /// They are used to persist events via the command handler outbox mechanism
    /// </remarks>
    protected void SaveDomainEvents(params IDomainEvent[] domainEvents)
    {
        _events.AddRange(domainEvents);
    }
}