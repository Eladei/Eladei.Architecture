using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

/// <summary>
/// Book information
/// </summary>
[Index(nameof(Name), nameof(Author), IsUnique = true)]
public class BookInformation : EntityBase
{
    /// <summary>
    /// Unique book identifier
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    [Required]
    [MaxLength(400)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Author
    /// </summary>
    [Required]
    public string Author { get; set; } = null!;

    /// <summary>
    /// Number of pages
    /// </summary>
    public uint? Pages { get; set; }

    /// <summary>
    /// Print run (circulation)
    /// </summary>
    public uint? Circulation { get; set; }

    /// <summary>
    /// Annotation / summary
    /// </summary>
    [MaxLength(1000)]
    public string? Annotation { get; set; } = null!;

    /// <summary>
    /// Editor
    /// </summary>
    [MaxLength(100)]
    public string? Editor { get; set; }

    /// <summary>
    /// Translator
    /// </summary>
    [MaxLength(100)]
    public string? Translator { get; set; }

    /// <summary>
    /// Illustrator / artist
    /// </summary>
    [MaxLength(100)]
    public string? Artist { get; set; }
}