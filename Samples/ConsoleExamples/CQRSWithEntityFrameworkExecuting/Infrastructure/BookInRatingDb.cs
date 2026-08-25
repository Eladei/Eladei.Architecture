using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

[Index(nameof(Name), nameof(Author), IsUnique = true)]
public sealed class BookInRatingDb
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Author { get; set; } = null!;

    public uint Votes { get; set; }
}
