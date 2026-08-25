namespace Eladei.BookRating.IntegrationTests;

public record NpgsqlConnectionParams
{
    public string ConnectionString { get; init; } = null!;
}
