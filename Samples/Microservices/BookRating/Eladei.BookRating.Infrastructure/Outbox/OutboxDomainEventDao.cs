using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Служба сохранения доменных событий в outbox
/// </summary>
public sealed class OutboxDomainEventDao : IEfOutboxDomainEventDao<BookRatingDbContext> {
    private readonly IIntegrationEventFactory _integrationEventFactory;
    private readonly ICorrelationContext _correlationContext;

    /// <summary>
    /// Создает объект класса DomainEventDao
    /// </summary>
    /// <param name="integrationEventFactory">Фабрика для формирования событий интеграции</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OutboxDomainEventDao(
        IIntegrationEventFactory integrationEventFactory,
        ICorrelationContext correlationContext) {
        _integrationEventFactory = integrationEventFactory
            ?? throw new ArgumentNullException(nameof(integrationEventFactory));

        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    public Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, BookRatingDbContext context, CancellationToken cancellationToken) {
        var command = new SaveDomainEventsToOutboxCommand(domainEvents, _integrationEventFactory, _correlationContext);

        return command.ExecuteAsync(context, cancellationToken);
    }
}