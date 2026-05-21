using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Service for saving domain events to the outbox
/// </summary>
public sealed class OutboxDomainEventDao : IEfOutboxDomainEventDao<BookRatingDbContext>
{
    private readonly IIntegrationEventFactory _integrationEventFactory;
    private readonly ICorrelationContext _correlationContext;

    /// <summary>
    /// Creates an instance of the <see cref="OutboxDomainEventDao"/> class
    /// </summary>
    /// <param name="integrationEventFactory">Factory used to convert domain events into 
    /// integration events suitable for external messaging systems.</param>
    /// <param name="correlationContext">Provides correlation information (e.g. CorrelationId) 
    /// used for distributed tracing across services and messages.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="integrationEventFactory"/> or
    /// <paramref name="correlationContext"/> is null.
    /// </exception>
    public OutboxDomainEventDao(
        IIntegrationEventFactory integrationEventFactory,
        ICorrelationContext correlationContext)
    {
        _integrationEventFactory = integrationEventFactory
            ?? throw new ArgumentNullException(nameof(integrationEventFactory));

        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    /// <inheritdoc />
    public Task SaveAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var command = new SaveDomainEventsToOutboxCommand(domainEvents, _integrationEventFactory, _correlationContext);

        return command.ExecuteAsync(context, cancellationToken);
    }
}