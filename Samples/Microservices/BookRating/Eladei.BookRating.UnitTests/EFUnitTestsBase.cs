using Microsoft.EntityFrameworkCore;
using Moq;

namespace Eladei.BookRating.UnitTests;

public abstract class EFUnitTestsBase<T> where T : DbContext
{
    protected readonly T _context;
    protected readonly Mock<T> _contextMock;

    public EFUnitTestsBase()
    {
        _contextMock = CreateContextMock();

        _context = ConfigureContext(_contextMock);
    }

    protected virtual Mock<T> CreateContextMock()
    {
        var options = new DbContextOptionsBuilder<T>().Options;

        return new Mock<T>(options);
    }

    protected abstract T ConfigureContext(Mock<T> contextMock);
}
