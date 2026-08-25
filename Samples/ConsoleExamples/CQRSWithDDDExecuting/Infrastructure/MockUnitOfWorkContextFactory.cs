using Eladei.Architecture.Cqrs.Ddd;

namespace CqrsWithDddExecuting.Infrastructure;

public class MockUnitOfWorkContextFactory : IUnitOfWorkContextFactory
{
    private static List<BookInRatingDb> DataContext = [];

    /// <inheritdoc />
    public IUnitOfWorkContext CreateContext()
        => new MockBookRatingUnitOfWorkContext(DataContext);
}
