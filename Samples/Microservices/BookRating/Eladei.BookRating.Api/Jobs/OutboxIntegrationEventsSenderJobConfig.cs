namespace Eladei.BookRating.Api.Jobs;

/// <summary>
/// Configuration for the job that sends integration events from the outbox
/// </summary>
public sealed record OutboxIntegrationEventsSenderJobConfig
{
    /// <summary>
    /// Time window (in seconds) for reserving integration events for sending
    /// </summary>
    public uint ReservingTimeInSeconds { get; init; }

    /// <summary>
    /// Maximum number of events to reserve for sending
    /// </summary>
    public uint MaxEventsToReserve { get; init; }
}