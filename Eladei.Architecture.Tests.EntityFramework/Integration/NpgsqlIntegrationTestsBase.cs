using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Eladei.Architecture.Tests.EntityFramework.Integration;

/// <summary>
/// Base class for PostgreSQL integration tests using Entity Framework.
/// Provides database creation, initialization, and cleanup lifecycle.
/// </summary>
/// <typeparam name="T">DbContext type</typeparam>
public abstract class NpgsqlIntegrationTestsBase<T> : IAsyncLifetime where T : DbContext
{
    private readonly NpgsqlConnectionParams _serverConnectionParams;
    private readonly Func<DbContextOptions<T>, T> _contextFactory;

    private string _dbConnectionString;
    private DbContextOptions<T> _contextOptions;

    /// <summary>
    /// Creates an instance of the integration test base class
    /// </summary>
    /// <param name="serverConnectionParams">PostgreSQL server connection parameters</param>
    /// <param name="contextFactory">Factory for creating DbContext instances</param>
    public NpgsqlIntegrationTestsBase(NpgsqlConnectionParams serverConnectionParams, Func<DbContextOptions<T>, T> contextFactory)
    {
        _serverConnectionParams = serverConnectionParams
            ?? throw new ArgumentNullException(nameof(serverConnectionParams));

        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        _contextOptions = await TestNpgsqlDatabaseFactory.CreateDatabaseAsync(
            _serverConnectionParams.ConnectionString, _contextFactory);

        using var context = CreateContext();
        _dbConnectionString = context.Database.GetConnectionString()!;

        await SetDataAsync(context);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await TestNpgsqlDatabaseFactory.DropDatabaseAsync(_dbConnectionString!);
    }

    /// <summary>
    /// Creates a new DbContext instance using the configured options
    /// </summary>
    public T CreateContext() => _contextFactory(_contextOptions);

    /// <summary>
    /// Seeds initial data into the test database.
    /// Can be overridden in derived test classes.
    /// </summary>
    /// <param name="context">DbContext instance</param>
    public virtual Task SetDataAsync(T context) => Task.CompletedTask;
}