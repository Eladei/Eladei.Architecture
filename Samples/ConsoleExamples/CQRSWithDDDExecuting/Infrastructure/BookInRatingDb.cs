namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Book information in the rating
/// </summary>
public sealed class BookInRatingDb
{
    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Book name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; set; } = null!;

    /// <summary>
    /// Votes cast for the book
    /// </summary>
    public uint Votes { get; set; }
}