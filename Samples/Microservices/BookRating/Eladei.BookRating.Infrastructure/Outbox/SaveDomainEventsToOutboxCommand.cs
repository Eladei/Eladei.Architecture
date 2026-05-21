using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities.IntegrationEvents;
using System.Text.Json;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Command for saving domain events to the outbox
/// </summary>
public sealed class SaveDomainEventsToOutboxCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly IIntegrationEventFactory _integrationEventFactory;
    private readonly IReadOnlyCollection<IDomainEvent> _domainEvents;
    private readonly ICorrelationContext _correlationContext;

    /// <summary>
    /// Creates an instance of the SaveDomainEventsToOutboxCommand class
    /// </summary>
    /// <param name="domainEvents">Domain events</param>
    /// <param name="integrationEventFactory">Factory for creating integration events</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SaveDomainEventsToOutboxCommand(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        IIntegrationEventFactory integrationEventFactory,
        ICorrelationContext correlationContext)
    {
        _domainEvents = domainEvents
            ?? throw new ArgumentNullException(nameof(domainEvents));

        _integrationEventFactory = integrationEventFactory
            ?? throw new ArgumentNullException(nameof(integrationEventFactory));

        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    /// <inheritdoc />
    public override Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var integrationEvents = new List<IIntegrationEvent>();

        foreach (var domainEvent in _domainEvents)
        {
            var integrationEvent = _integrationEventFactory.Create(domainEvent, _correlationContext.CorrelationId);

            if (integrationEvent is not null)
                integrationEvents.Add(integrationEvent);
        }

        if (integrationEvents.Count != 0)
            context.IntegrationEvents.AddRange(integrationEvents.Select(Convert));

        return Task.CompletedTask;
    }

    private static IntegrationEventToSend Convert(IIntegrationEvent integrationEvent)
    {
        var eventType = integrationEvent.GetType();

        var metadata = JsonSerializer.Serialize(integrationEvent, eventType);

        return new IntegrationEventToSend
        {
            Id = integrationEvent.EventId,
            EntityId = integrationEvent.EntityId,
            CorrelationId = integrationEvent.CorrelationId,
            EventType = eventType.AssemblyQualifiedName!,
            EventMetadata = metadata
        };
    }
}