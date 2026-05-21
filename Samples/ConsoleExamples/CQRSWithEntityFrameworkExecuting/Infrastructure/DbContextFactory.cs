using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Factory for the unit-of-work database context
/// </summary>
internal class DbContextFactory : IDbContextFactory<BookRatingDbContext>
{
    public BookRatingDbContext CreateDbContext()
    {
        var context = new BookRatingDbContext();

        context.Database.EnsureCreated();

        context.Books.Load();

        return context;
    }
}