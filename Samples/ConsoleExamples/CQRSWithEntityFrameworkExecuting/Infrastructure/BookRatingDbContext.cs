using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

public class BookRatingDbContext : DbContext
{
    public DbSet<BookInRatingDb> Books { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=bookRating.db");
    }
}
