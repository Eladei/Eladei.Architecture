namespace Eladei.Architecture.Tests.EntityFramework.Integration;

/// <summary>
/// PostgreSQL connection parameters
/// </summary>
public record NpgsqlConnectionParams
{
    /// <summary>
    /// Connection string
    /// </summary>
    public string ConnectionString { get; init; } = null!;
}