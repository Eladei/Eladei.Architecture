using Eladei.Architecture.Cqrs.Ddd;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Мок фабрики контекста единицы работы
/// </summary>
public class MockUnitOfWorkContextFactory : IUnitOfWorkContextFactory
{
    private static List<BookInRatingDb> DataContext = [];

    public IUnitOfWorkContext CreateContext()
        => new MockBookRatingUnitOfWorkContext(DataContext);
}