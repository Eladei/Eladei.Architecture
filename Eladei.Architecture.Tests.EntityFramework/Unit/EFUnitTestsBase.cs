using Microsoft.EntityFrameworkCore;
using Moq;

namespace Eladei.Architecture.Tests.EntityFramework.Unit;

public abstract class EFUnitTestsBase<T> where T : DbContext
{
    protected T _context;

    public EFUnitTestsBase()
    {
        _context = SetUpDbContext(new Mock<T>());
    }

    protected abstract T SetUpDbContext(Mock<T> contextMock);
}