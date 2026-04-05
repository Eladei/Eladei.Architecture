using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Команда резервирования событий интеграции в outbox для последующей отправки
/// </summary>
public sealed class ReserveIntegrationEventsInOutboxForSendingCommand : EfCommandWithResultBase<BookRatingDbContext, int> {
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly uint _maxEventsToReserve;

    /// <summary>
    /// Создает объект класса ReserveIntegrationEventsForSendingCommand
    /// </summary>
    /// <param name="senderId">Идентификатор службы, отправляющей события</param>
    /// <param name="reservingSpanSeconds">Время резервирования в секундах</param>
    /// <param name="maxEventsToReserve">Максимальное количество событий для резервирования</param>
    public ReserveIntegrationEventsInOutboxForSendingCommand(Guid senderId, uint reservingSpanSeconds, uint maxEventsToReserve) {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;
        _maxEventsToReserve = maxEventsToReserve;
    }

    public override async Task<int> ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken) {
        var reservingDate = DateTime.UtcNow;

        var eventsToReserve = await context.IntegrationEvents
            .Where(x => !x.IsSent
                && (x.ReservedAt == null 
                    || reservingDate >= x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds)))
            .OrderBy(x => x.CreatedAtUtc)
            .Take((int)_maxEventsToReserve)
            .ToArrayAsync(cancellationToken);

        foreach(var evnt in eventsToReserve) {
            evnt.ReservedAt = reservingDate;
            evnt.ReservedBy = _senderId;
        };

        return eventsToReserve.Length;
    }
}