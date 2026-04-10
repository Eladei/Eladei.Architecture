using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities.IntegrationEvents;

/// <summary>
/// Информация об отправке события интеграции
/// </summary>
public class IntegrationEventToSend : EntityBase
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Id сущности, к которой привязано событие
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Id для сквозного отслеживания
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Тип события
    /// </summary>
    [Required]
    public string EventType { get; set; } = null!;

    /// <summary>
    /// Метаданные события для парсинга
    /// </summary>
    [Required]
    public string EventMetadata { get; set; } = null!;

    /// <summary>
    /// Показатель успешности отправки
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Число попыток отправки
    /// </summary>
    public int NumberOfSendingAttempts { get; set; }

    /// <summary>
    /// Дата отправки
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Последняя ошибка отправки
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Идентификатор системы, зазерезервировавшей событие для отправки
    /// </summary>
    public Guid? ReservedBy { get; set; }

    /// <summary>
    /// Дата резервирования события для отправки
    /// </summary>
    public DateTime? ReservedAt { get; set; }
}