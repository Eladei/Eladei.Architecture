using Microsoft.EntityFrameworkCore;
using Moq;

namespace Eladei.Architecture.Tests.EntityFramework.Unit;

/// <summary>
/// Base class for Entity Framework unit tests.
/// Provides a mocked DbContext setup for isolated testing without a real database.
/// </summary>
/// <typeparam name="T">DbContext type</typeparam>
public abstract class EFUnitTestsBase<T> where T : DbContext
{
    /// <summary>
    /// The mocked or configured DbContext instance used in tests
    /// </summary>
    protected T _context;

    /// <summary>
    /// Initializes a new instance of the unit test base class
    /// and sets up the DbContext using a mock.
    /// </summary>
    public EFUnitTestsBase()
    {
        _context = SetUpDbContext(new Mock<T>());
    }

    /// <summary>
    /// Configures and builds the DbContext instance from a mocked context
    /// </summary>
    /// <param name="contextMock">Mock of DbContext</param>
    /// <returns>Configured DbContext instance for testing</returns>
    protected abstract T SetUpDbContext(Mock<T> contextMock);
}