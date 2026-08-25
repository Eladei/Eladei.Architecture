using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Outbox service for storing integration events
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <remarks>
/// Used when integration events need to be persisted in the database
/// and later published in a separate process (e.g., a background job).
/// Events must be saved within the same transaction as other data.
/// For this purpose, pass the same database context to the SaveAsync method.
/// </remarks>
public interface IEfOutboxIntegrationEventWriter<T> where T : DbContext
{
    /// <summary>
    /// Saves a integration event to persistent storage
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SaveAsync(IReadOnlyCollection<IIntegrationEvent> integrationEvents, T context, CancellationToken cancellationToken);
}
