using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Model.Entities.Outbox;
using System.Text.Json;

namespace Eladei.BookRating.Infrastructure.Outbox;

public sealed class OutboxMessageFactory : IOutboxMessageFactory
{
    private readonly ICorrelationContext _correlationContext;

    public OutboxMessageFactory(ICorrelationContext correlationContext)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    public OutboxMessage Create(IIntegrationEvent integrationEvent) =>
        integrationEvent switch
        {
            BookWasRegisteredInRatingIntegrationEvent evnt => Convert(evnt, () => evnt.BookId),
            BookInfoWasUpdatedInRatingIntegrationEvent evnt => Convert(evnt, () => evnt.BookId),
            BookWasRemovedFromRatingIntegrationEvent evnt => Convert(evnt, () => evnt.BookId),
            _ => throw new ArgumentException(
                $"Unsupported integration event type: {integrationEvent.GetType().FullName}")
        };

    private OutboxMessage Convert(IIntegrationEvent integrationEvent, Func<Guid>? messageKeyProvider = null)
    {
        var eventType = integrationEvent.GetType();

        var metadata = JsonSerializer.Serialize(integrationEvent, eventType);

        var messageKey = messageKeyProvider is null
            ? Guid.NewGuid()
            : messageKeyProvider();

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            MessageKey = messageKey,
            CorrelationId = _correlationContext.CorrelationId,
            EventType = eventType.AssemblyQualifiedName!,
            EventMetadata = metadata
        };
    }
}
