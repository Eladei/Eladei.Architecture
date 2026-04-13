namespace Eladei.Architecture.Tests.EntityFramework.Integration;

/// <summary>
/// Параметры соединения с сервером PostgreSQL
/// </summary>
public record NpgsqlConnectionParams
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    public string ConnectionString { get; init; } = null!;
}