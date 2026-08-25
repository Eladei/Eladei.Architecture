using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

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
