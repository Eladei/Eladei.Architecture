using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Eladei.BookRating.Infrastructure.Outbox;

public sealed class SendOutboxMessagesCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly IIntegrationEventBus _integrationEventBus;

    /// <exception cref="ArgumentNullException"></exception>
    public SendOutboxMessagesCommand(
        Guid senderId,
        uint reservingSpanSeconds,
        IIntegrationEventBus integrationEventBus)
    {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var sendingDate = DateTime.UtcNow;

        var eventsToSend = await context.OutboxMessages
            .Where(x => !x.IsSent
                && x.ReservedBy == _senderId
                && x.ReservedAt != null
                && sendingDate < x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds))
            .OrderBy(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var sendEvents = true;

        foreach (var evnt in eventsToSend)
        {
            try
            {
                evnt.NumberOfSendingAttempts += 1;

                var messageInfo = GetMessageInfo(evnt);

                if (sendEvents)
                {
                    await _integrationEventBus.PublishEventAsync(
                        messageInfo.IntegrationEvent, messageInfo.Headers);

                    evnt.IsSent = true;
                    evnt.SentAt = sendingDate;
                }
            }
            catch (Exception ex)
            {
                evnt.LastError = ex.Message;
                sendEvents = false;
            }

            evnt.ReservedBy = null;
        }
    }

    private static OutboxMessageInfo GetMessageInfo(OutboxMessage eventDb)
    {
        var eventType = Type.GetType(eventDb.EventType)
            ?? throw new Exception("Type resolution error");

        var result = JsonSerializer.Deserialize(eventDb.EventMetadata, eventType)
            ?? throw new Exception("Deserialization error");

        return new OutboxMessageInfo
        {
            Headers = new Dictionary<string, string>
            {
                [IIntegrationEventBus.IntegrationEventHeadersNames.EventId] = eventDb.Id.ToString(),
                [IIntegrationEventBus.IntegrationEventHeadersNames.MessageKey] = eventDb.MessageKey.ToString(),
                [IIntegrationEventBus.IntegrationEventHeadersNames.CorrelationId] = eventDb.CorrelationId.ToString()
            },
            IntegrationEvent = (IIntegrationEvent)result
        };
    }

    private record OutboxMessageInfo
    {
        public Dictionary<string, string> Headers { get; init; } = null!;

        public IIntegrationEvent IntegrationEvent { get; init; } = null!;
    }
}
