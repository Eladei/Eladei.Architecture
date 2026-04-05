namespace Eladei.BookInfo.Domain.Queries.ReadModel;

/// <summary>
/// Информация о книге
/// </summary>
public record BookInfoReadModel {
    /// <summary>
    /// Уникальный идентификатор книги
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Название 
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; init; } = null!;

    /// <summary>
    /// Число страниц
    /// </summary>
    public uint? Pages { get; init; }

    /// <summary>
    /// Тираж
    /// </summary>
    public uint? Circulation { get; init; }

    /// <summary>
    /// Аннотация
    /// </summary>
    public string? Annotation { get; init; } = null!;

    /// <summary>
    /// Редактор
    /// </summary>
    public string? Editor { get; init; }

    /// <summary>
    /// Переводчик
    /// </summary>
    public string? Translator { get; init; }

    /// <summary>
    /// Художник
    /// </summary>
    public string? Artist { get; init; }
}