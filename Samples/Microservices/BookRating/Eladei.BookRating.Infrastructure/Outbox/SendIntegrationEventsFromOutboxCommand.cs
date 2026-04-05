using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Команда отправки событий интеграции из outbox
/// </summary>
public sealed class SendIntegrationEventsFromOutboxCommand : EfCommandBase<BookRatingDbContext> {
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly IIntegrationEventBus _integrationEventBus;

    /// <summary>
    /// Создает объект класса SendIntegrationEventsCommand
    /// </summary>
    /// <param name="integrationEventBus">Шина событий интеграции</param>
    /// <exception cref="ArgumentException"></exception>
    public SendIntegrationEventsFromOutboxCommand(Guid senderId, uint reservingSpanSeconds, IIntegrationEventBus integrationEventBus) {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));
    }

    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken) {
        var sendingDate = DateTime.UtcNow;

        var eventsToSend = await context.IntegrationEvents
            .Where(x => !x.IsSent
                && x.ReservedBy == _senderId
                && x.ReservedAt != null
                    && sendingDate < x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds))
            .OrderBy(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var sendEvents = true;

        foreach (var evnt in eventsToSend) {
            try {
                evnt.NumberOfSendingAttempts += 1;

                var integrationEvent = Map(evnt);

                if (sendEvents) {
                    await _integrationEventBus.PublishEventAsync(integrationEvent);
                    evnt.IsSent = true;
                    evnt.SentAt = sendingDate;
                }
            }
            catch(Exception ex) {
                evnt.LastError = ex.Message;

                sendEvents = false;
            }

            evnt.ReservedBy = null;
        }
    }

    private static IIntegrationEvent Map(IntegrationEventToSend eventDb) {
        var eventType = Type.GetType(eventDb.EventType) ?? throw new Exception("Ошибка");

        var result = JsonSerializer.Deserialize(eventDb.EventMetadata, eventType) ?? throw new Exception("Ошибка");

        return (IIntegrationEvent)result;
    }
}