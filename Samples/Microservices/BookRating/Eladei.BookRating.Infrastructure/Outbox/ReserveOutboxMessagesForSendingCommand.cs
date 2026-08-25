using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Infrastructure.Outbox;

public sealed class ReserveOutboxMessagesForSendingCommand : EfCommandWithResultBase<BookRatingDbContext, int>
{
    private readonly Guid _senderId;
    private readonly uint _reservingSpanSeconds;
    private readonly uint _maxEventsToReserve;

    public ReserveOutboxMessagesForSendingCommand(Guid senderId, uint reservingSpanSeconds, uint maxEventsToReserve)
    {
        _senderId = senderId;
        _reservingSpanSeconds = reservingSpanSeconds;
        _maxEventsToReserve = maxEventsToReserve;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var reservingDate = DateTime.UtcNow;

        var eventsToReserve = await context.OutboxMessages
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
