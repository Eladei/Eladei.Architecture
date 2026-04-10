using Microsoft.EntityFrameworkCore;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Фабрика контекста единицы работы
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