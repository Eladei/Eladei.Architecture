using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Infrastructure.Outbox;

/// <summary>
/// Command for reserving integration events in the outbox for later sending
/// </summary>
public sealed class ReserveIntegrationEventsInOutboxForSendingCommand : EfCommandWithResultBase<BookRatingDbContext, int>
{
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly uint _maxEventsToReserve;

    /// <summary>
    /// Creates an instance of the ReserveIntegrationEventsForSendingCommand class
    /// </summary>
    /// <param name="senderId">Identifier of the service sending events</param>
    /// <param name="reservingSpanSeconds">Reservation time in seconds</param>
    /// <param name="maxEventsToReserve">Maximum number of events to reserve</param>
    public ReserveIntegrationEventsInOutboxForSendingCommand(Guid senderId, uint reservingSpanSeconds, uint maxEventsToReserve)
    {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;
        _maxEventsToReserve = maxEventsToReserve;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var reservingDate = DateTime.UtcNow;

        var eventsToReserve = await context.IntegrationEvents
            .Where(x => !x.IsSent
                && (x.ReservedAt == null
                    || reservingDate >= x.ReservedAt.Value.AddSeconds(_reservingSpanSeconds)))
            .OrderBy(x => x.CreatedAtUtc)
            .Take((int)_maxEventsToReserve)
            .ToArrayAsync(cancellationToken);

        foreach (var evnt in eventsToReserve)
        {
            evnt.ReservedAt = reservingDate;
            evnt.ReservedBy = _senderId;
        }

        return eventsToReserve.Length;
    }
}