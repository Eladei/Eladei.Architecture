using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Outbox service for storing domain events
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <remarks>
/// Used when domain events need to be persisted in the database
/// and later published in a separate process (e.g., a background job).
/// Events must be saved within the same transaction as other data.
/// For this purpose, pass the same database context to the SaveAsync method.
/// </remarks>
public interface IEfOutboxDomainEventDao<T> where T : DbContext
{
    /// <summary>
    /// Saves a domain event to persistent storage
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, T context, CancellationToken cancellationToken);
}