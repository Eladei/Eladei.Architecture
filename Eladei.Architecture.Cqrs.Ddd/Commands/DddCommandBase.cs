using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Base class for commands
/// </summary>
public abstract class DddCommandBase : IDddCommand
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// Domain events raised by the command
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events;

    /// <summary>
    /// Clears all domain events
    /// </summary>
    public void ClearEvents()
    {
        _events.Clear();
    }

    /// <summary>
    /// Executes logic before command execution
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>
    /// True if execution can continue; otherwise false
    /// </returns>
    public virtual Task<bool> BeforeExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public abstract Task ExecuteAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds domain events
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <remarks>
    /// Added domain events are available via the <see cref="Events"/> collection.
    /// They are used to persist events via the command handler outbox mechanism
    /// </remarks>
    protected void AddDomainEvents(params IDomainEvent[] domainEvents)
    {
        _events.AddRange(domainEvents);
    }
}