using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Контекст рейтинга книг
/// </summary>
public class BookRatingDbContext : DbContext {
    /// <summary>
    /// Книги
    /// </summary>
    public DbSet<BookInRatingDb> Books { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=bookRating.db");
    }
}