namespace CqrsWithDddExecuting.ReadModel;

public sealed record class BookInRatingReadModel
{
    public Guid BookId { get; init; }

    public string Name { get; init; } = null!;

    public string Author { get; init; } = null!;

    public uint Votes { get; init; }
}
