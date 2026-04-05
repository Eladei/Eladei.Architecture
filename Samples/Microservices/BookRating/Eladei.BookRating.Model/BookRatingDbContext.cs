using Eladei.BookRating.Model.Entities;
using Eladei.BookRating.Model.Entities.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eladei.BookRating.Model;

/// <summary>
/// Контекст базы данных для работы с рейтингом книг
/// </summary>
public class BookRatingDbContext : DbContext {
    public BookRatingDbContext() : base() { }

    public BookRatingDbContext(DbContextOptions<BookRatingDbContext> contextOptions) : base(contextOptions) { }

    /// <summary>
    /// Информация о книгах
    /// </summary>
    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<IntegrationEventToSend> IntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .Property(e => e.Version).IsRowVersion();
        modelBuilder.Entity<IntegrationEventToSend>()
            .Property(e => e.Version).IsRowVersion();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        foreach (var entry in ChangeTracker.Entries()) {
            switch (entry.State) {
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