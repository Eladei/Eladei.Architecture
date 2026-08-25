using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities;

[Index(nameof(Name), nameof(Author), IsUnique = true)]
public sealed class Book : EntityBase
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(400)]
    public string Name { get; set; } = null!;

    [Required]
    public string Author { get; set; } = null!;

    public uint Votes { get; set; }
}
