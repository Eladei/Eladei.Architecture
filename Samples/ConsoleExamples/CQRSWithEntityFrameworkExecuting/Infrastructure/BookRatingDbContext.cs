using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Book rating database context
/// </summary>
public class BookRatingDbContext : DbContext
{
    /// <summary>
    /// Books
    /// </summary>
    public DbSet<BookInRatingDb> Books { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=bookRating.db");
    }
}