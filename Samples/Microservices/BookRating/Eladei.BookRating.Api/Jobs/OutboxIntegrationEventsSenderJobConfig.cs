namespace Eladei.BookRating.Api.Jobs; 

/// <summary>
/// Конфигурация job отправки событий интеграции из outbox
/// </summary>
public sealed record OutboxIntegrationEventsSenderJobConfig {
    /// <summary>
    /// Время резервирования job события интеграции для отправки
    /// </summary>
    public uint ReservingTimeInSeconds { get; init; }

    /// <summary>
    /// Максимальное количество резервируемых событий для отправки
    /// </summary>
    public uint MaxEventsToReserve { get; init; }
}