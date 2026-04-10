using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities.IntegrationEvents;
using System.Text.Json;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Команда сохранения доменных событий в outbox
/// </summary>
public sealed class SaveDomainEventsToOutboxCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly IIntegrationEventFactory _integrationEventFactory;
    private readonly IReadOnlyCollection<IDomainEvent> _domainEvents;
    private readonly ICorrelationContext _correlationContext;

    /// <summary>
    /// Создает объект класса SaveDomainEventsCommand
    /// </summary>
    /// <param name="domainEvents">События предметной области</param>
    /// <param name="integrationEventFactory">Фабрика для формирования событий интеграции</param>
    /// <param name="correlationContext">Контекст корреляции</param>
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