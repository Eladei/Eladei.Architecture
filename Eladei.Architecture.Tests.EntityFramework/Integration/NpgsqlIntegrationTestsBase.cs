using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Eladei.Architecture.Tests.EntityFramework.Integration;

public abstract class NpgsqlIntegrationTestsBase<T> : IAsyncLifetime where T : DbContext
{
    private readonly NpgsqlConnectionParams _serverConnectionParams;
    private readonly Func<DbContextOptions<T>, T> _contextFactory;

    private string _dbConnectionString;
    private DbContextOptions<T> _contextOptions;

    public NpgsqlIntegrationTestsBase(NpgsqlConnectionParams serverConnectionParams, Func<DbContextOptions<T>, T> contextFactory)
    {
        _serverConnectionParams = serverConnectionParams
            ?? throw new ArgumentNullException(nameof(serverConnectionParams));

        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async ValueTask InitializeAsync()
    {
        _contextOptions = await TestNpgsqlDatabaseFactory.CreateDatabaseAsync(
            _serverConnectionParams.ConnectionString, _contextFactory);

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