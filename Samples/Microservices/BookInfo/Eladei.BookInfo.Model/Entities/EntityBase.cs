using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

public abstract class EntityBase
{
    public uint Version { get; set; }

    [Required]
    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ModifiedAtUtc { get; set; }
}
