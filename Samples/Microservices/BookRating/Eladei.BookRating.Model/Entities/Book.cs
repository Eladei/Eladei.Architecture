using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities;

/// <summary>
/// Book
/// </summary>
[Index(nameof(Name), nameof(Author), IsUnique = true)]
public class Book : EntityBase
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
    /// Number of votes cast for the book
    /// </summary>
    public uint Votes { get; set; }
}