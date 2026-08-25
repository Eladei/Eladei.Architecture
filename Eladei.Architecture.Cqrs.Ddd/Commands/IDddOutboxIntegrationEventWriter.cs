using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Outbox service for persisting domain events
/// </summary>
/// <remarks>
/// Used when integration events must be stored in the database
/// and later published in a separate process, such as a background job.
/// Events should be saved within the same transaction as other data.
/// To achieve this, pass the same repository factory instance
/// that is used by the unit of work
/// </remarks>
public interface IDddOutboxIntegrationEventWriter
{
    /// <summary>
    /// Persists integration events to storage
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SaveAsync(
        IReadOnlyCollection<IIntegrationEvent> integrationEvents,
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken);
}
