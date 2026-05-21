using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities.IntegrationEvents;

/// <summary>
/// Information about sending an integration event
/// </summary>
public class IntegrationEventToSend : EntityBase
{
    /// <summary>
    /// Event identifier
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the entity associated with the event
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Id for distributed tracing
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Event type
    /// </summary>
    [Required]
    public string EventType { get; set; } = null!;

    /// <summary>
    /// Event metadata for parsing
    /// </summary>
    [Required]
    public string EventMetadata { get; set; } = null!;

    /// <summary>
    /// Sending success indicator
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Number of sending attempts
    /// </summary>
    public int NumberOfSendingAttempts { get; set; }

    /// <summary>
    /// Sending date
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Last sending error
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Identifier of the system that reserved the event for sending
    /// </summary>
    public Guid? ReservedBy { get; set; }

    /// <summary>
    /// Date when the event was reserved for sending
    /// </summary>
    public DateTime? ReservedAt { get; set; }
}