using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Book information in the rating system
/// </summary>
[Index(nameof(Name), nameof(Author), IsUnique = true)]
public sealed class BookInRatingDb
{
    /// <summary>
    /// Book identifier
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Book title
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; set; } = null!;

    /// <summary>
    /// Number of votes for the book
    /// </summary>
    public uint Votes { get; set; }
}