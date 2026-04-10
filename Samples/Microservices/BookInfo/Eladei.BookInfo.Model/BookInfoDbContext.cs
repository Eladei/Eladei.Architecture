using Eladei.BookInfo.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Model;

/// <summary>
/// Контекст базы данных для работы с информацией о книгах
/// </summary>
public class BookInfoDbContext : DbContext
{
    public BookInfoDbContext() : base() { }

    public BookInfoDbContext(DbContextOptions<BookInfoDbContext> options) : base(options) { }

    /// <summary>
    /// Информация о книгах
    /// </summary>
    public DbSet<BookInformation> BookInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookInformation>()
            .Property(p => p.Version).IsRowVersion();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((EntityBase)entry.Entity).CreatedAtUtc = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    ((EntityBase)entry.Entity).ModifiedAtUtc = DateTime.UtcNow;
                    break;
                default:
                    break;
            }
            ;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}