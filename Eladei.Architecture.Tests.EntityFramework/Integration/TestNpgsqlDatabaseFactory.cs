using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Eladei.Architecture.Tests.EntityFramework.Integration;

public static class TestNpgsqlDatabaseFactory
{
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

        // Завершаем активные подключения
        using (var terminateCmd = connection.CreateCommand())
        {
            terminateCmd.CommandText = $@"
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{dbName}' AND pid <> pg_backend_pid();";

            await terminateCmd.ExecuteNonQueryAsync();
        }

        // Удаляем БД
        using var dropCmd = connection.CreateCommand();
        dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\"";

        await dropCmd.ExecuteNonQueryAsync();
    }
}