using Eladei.BookInfo.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Model;

/// <summary>
/// Database context for working with book information
/// </summary>
public class BookInfoDbContext : DbContext
{
    public BookInfoDbContext() : base() { }

    public BookInfoDbContext(DbContextOptions<BookInfoDbContext> options) : base(options) { }

    /// <summary>
    /// Book information
    /// </summary>
    public DbSet<BookInformation> BookInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookInformation>()
            .Property(p => p.Version)
            .IsRowVersion();
    }

    /// <inheritdoc />
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
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}