namespace Eladei.BookInfo.Domain.Queries.ReadModel;

/// <summary>
/// Book information
/// </summary>
public record BookInfoReadModel
{
    /// <summary>
    /// Unique book identifier
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
    /// Number of pages
    /// </summary>
    public uint? Pages { get; init; }

    /// <summary>
    /// Print run (circulation)
    /// </summary>
    public uint? Circulation { get; init; }

    /// <summary>
    /// Annotation
    /// </summary>
    public string? Annotation { get; init; } = null!;

    /// <summary>
    /// Editor
    /// </summary>
    public string? Editor { get; init; }

    /// <summary>
    /// Translator
    /// </summary>
    public string? Translator { get; init; }

    /// <summary>
    /// Artist
    /// </summary>
    public string? Artist { get; init; }
}