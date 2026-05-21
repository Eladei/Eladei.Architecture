using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

/// <summary>
/// Base class for database entities
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public uint Version { get; set; }

    /// <summary>
    /// Creation date and time in UTC
    /// </summary>
    [Required]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Last modification date and time in UTC
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }
}