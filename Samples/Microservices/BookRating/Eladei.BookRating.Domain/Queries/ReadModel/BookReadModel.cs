namespace Eladei.BookRating.Domain.Queries.ReadModel;

/// <summary>
/// Book information
/// </summary>
public record BookReadModel
{
    /// <summary>
    /// Book identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Title
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; init; } = null!;

    /// <summary>
    /// Number of votes cast for the book
    /// </summary>
    public uint Votes { get; init; }

    /// <summary>
    /// Book registration date and time in UTC
    /// </summary>
    public DateTime RegisteredAtUtc { get; init; }
}