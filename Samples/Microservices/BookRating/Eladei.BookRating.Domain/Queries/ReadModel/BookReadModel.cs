namespace Eladei.BookRating.Domain.Queries.ReadModel;

/// <summary>
/// Информация о книге
/// </summary>
public record BookReadModel
{
    /// <summary>
    /// Идентификатор книги
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
    /// Количество голосов, отданных за книгу
    /// </summary>
    public uint Votes { get; init; }

    /// <summary>
    /// Дата и время регистрации книги в стандарте UTC
    /// </summary>
    public DateTime RegisteredAtUtc { get; init; }
}