using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Eladei.Architecture.Tests.EntityFramework.Integration;

/// <summary>
/// Factory for creating and managing PostgreSQL test databases.
/// Responsible for database creation, migration execution, and cleanup.
/// </summary>
public static class TestNpgsqlDatabaseFactory
{
    /// <summary>
    /// Creates a new isolated PostgreSQL database for integration testing
    /// and applies EF Core migrations.
    /// </summary>
    /// <typeparam name="TContext">DbContext type</typeparam>
    /// <param name="connectionString">Base PostgreSQL connection string</param>
    /// <param name="contextFactory">Factory for creating DbContext instances</param>
    /// <returns>Configured DbContext options for the created test database</returns>
    public static async Task<DbContextOptions<TContext>> CreateDatabaseAsync<TContext>(
        string connectionString,
        Func<DbContextOptions<TContext>, TContext> contextFactory)
        where TContext : DbContext
    {
        var dbName = "test_db_" + Guid.NewGuid().ToString("N");

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = dbName
        };

        var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();

        using (var connection = new NpgsqlConnection(masterConnectionString))
        {
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";

            await cmd.ExecuteNonQueryAsync();
        }

        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(builder.ToString())
            .Options;

        using (var context = contextFactory(options))
        {
            await context.Database.MigrateAsync();
        }

        return options;
    }

    /// <summary>
    /// Drops a previously created PostgreSQL test database.
    /// Terminates active connections before deletion.
    /// </summary>
    /// <param name="connectionString">Connection string pointing to the target database</param>
    public static async Task DropDatabaseAsync(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var dbName = builder.Database;

        var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();

        using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();

        // Terminate active connections to the database
        using (var terminateCmd = connection.CreateCommand())
        {
            terminateCmd.CommandText = $@"
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{dbName}' AND pid <> pg_backend_pid();";

            await terminateCmd.ExecuteNonQueryAsync();
        }

        // Drop database
        using var dropCmd = connection.CreateCommand();
        dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\"";

        await dropCmd.ExecuteNonQueryAsync();
    }
}