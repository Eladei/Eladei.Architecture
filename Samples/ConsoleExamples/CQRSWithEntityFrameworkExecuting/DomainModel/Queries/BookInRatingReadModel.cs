namespace CqrsWithEntityFrameworkExecuting.DomainModel.Queries;

/// <summary>
/// Book information in the rating
/// </summary>
public sealed record class BookInRatingReadModel
{
    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid BookId { get; init; }

    /// <summary>
    /// Book title
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; init; } = null!;

    /// <summary>
    /// Number of votes for the book
    /// </summary>
    public uint Votes { get; init; }
}