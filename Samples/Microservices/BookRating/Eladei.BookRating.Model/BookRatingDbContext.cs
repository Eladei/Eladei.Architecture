using Eladei.BookRating.Model.Entities;
using Eladei.BookRating.Model.Entities.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eladei.BookRating.Model;

public class BookRatingDbContext : DbContext
{
    public BookRatingDbContext(DbContextOptions<BookRatingDbContext> contextOptions) : base(contextOptions) { }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .Property(e => e.Version).IsRowVersion();
        modelBuilder.Entity<OutboxMessage>()
            .Property(e => e.Version).IsRowVersion();
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
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
