namespace Eladei.BookRating.Api.Jobs;

public sealed record OutboxIntegrationEventsSenderJobConfig
{
    public uint ReservingTimeInSeconds { get; init; }

    public uint MaxEventsToReserve { get; init; }
}
