using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities.Outbox;

public sealed class OutboxMessage : EntityBase
{
    [Key]
    public Guid Id { get; set; }

    public Guid MessageKey { get; set; }

    public Guid CorrelationId { get; set; }

    [Required]
    public string EventType { get; set; } = null!;

    [Required]
    public string EventMetadata { get; set; } = null!;

    public bool IsSent { get; set; }

    public int NumberOfSendingAttempts { get; set; }

    public DateTime? SentAt { get; set; }

    public string? LastError { get; set; }

    public Guid? ReservedBy { get; set; }

    public DateTime? ReservedAt { get; set; }
}
