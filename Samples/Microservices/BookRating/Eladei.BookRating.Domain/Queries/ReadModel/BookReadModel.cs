namespace Eladei.BookRating.Application.Queries.ReadModel;

public sealed record BookReadModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Author { get; init; } = null!;

    public uint Votes { get; init; }

    public DateTime RegisteredAtUtc { get; init; }
}
