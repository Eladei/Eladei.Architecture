namespace Eladei.BookInfo.Application.Queries.ReadModel;

public sealed record BookInfoReadModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Author { get; init; } = null!;

    public uint? Pages { get; init; }

    public uint? Circulation { get; init; }

    public string? Annotation { get; init; } = null!;

    public string? Editor { get; init; }

    public string? Translator { get; init; }

    public string? Artist { get; init; }
}
