using Eladei.Architecture.Cqrs.Ddd;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Mock Unit of Work context factory
/// </summary>
public class MockUnitOfWorkContextFactory : IUnitOfWorkContextFactory
{
    private static List<BookInRatingDb> DataContext = [];

    /// <inheritdoc />
    public IUnitOfWorkContext CreateContext()
        => new MockBookRatingUnitOfWorkContext(DataContext);
}