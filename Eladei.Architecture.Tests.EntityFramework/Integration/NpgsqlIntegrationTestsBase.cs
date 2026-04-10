using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Eladei.Architecture.Tests.EntityFramework.Integration;

public abstract class NpgsqlIntegrationTestsBase<T> : IAsyncLifetime where T : DbContext
{
    private readonly Func<DbContextOptions<T>, T> _contextFactory;

    private readonly string _serverConnectionString;
    private string _dbConnectionString;
    private DbContextOptions<T> _contextOptions;

    public NpgsqlIntegrationTestsBase(string serverConnectionString, Func<DbContextOptions<T>, T> contextFactory)
    {
        _serverConnectionString = serverConnectionString
            ?? throw new ArgumentNullException(nameof(serverConnectionString));

        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async ValueTask InitializeAsync()
    {
        _contextOptions = await TestNpgsqlDatabaseFactory.CreateDatabaseAsync(_serverConnectionString, _contextFactory);

        using var context = CreateContext();
        _dbConnectionString = context.Database.GetConnectionString()!;

        await SetDataAsync(context);
    }

    public async ValueTask DisposeAsync()
    {
        await TestNpgsqlDatabaseFactory.DropDatabaseAsync(_dbConnectionString!);
    }

    public T CreateContext() => _contextFactory(_contextOptions);

    public virtual Task SetDataAsync(T context) => Task.CompletedTask;
}