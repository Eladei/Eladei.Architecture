using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

[Index(nameof(Name), nameof(Author), IsUnique = true)]
public class BookInformation : EntityBase
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(400)]
    public string Name { get; set; } = null!;

    [Required]
    public string Author { get; set; } = null!;

    public uint? Pages { get; set; }

    public uint? Circulation { get; set; }

    [MaxLength(1000)]
    public string? Annotation { get; set; } = null!;

    [MaxLength(100)]
    public string? Editor { get; set; }

    [MaxLength(100)]
    public string? Translator { get; set; }

    [MaxLength(100)]
    public string? Artist { get; set; }
}
