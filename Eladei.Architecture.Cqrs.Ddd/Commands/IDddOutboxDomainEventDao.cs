using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Outbox service for persisting domain events
/// </summary>
/// <remarks>
/// Used when domain events must be stored in the database
/// and later published in a separate process, such as a background job.
/// Events should be saved within the same transaction as other data.
/// To achieve this, pass the same repository factory instance
/// that is used by the unit of work
/// </remarks>
public interface IDddOutboxDomainEventDao
{
    /// <summary>
    /// Persists domain events to storage
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SaveAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken);
}