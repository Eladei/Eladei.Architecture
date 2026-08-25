namespace CqrsWithDddExecuting.Infrastructure;

public sealed class BookInRatingDb
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Author { get; set; } = null!;

    public uint Votes { get; set; }
}
