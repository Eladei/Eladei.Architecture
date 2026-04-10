namespace CqrsWithDddExecuting.ReadModel;

/// <summary>
/// Информация о книге в рейтинге
/// </summary>
public sealed record class BookInRatingReadModel
{
    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId { get; init; }

    /// <summary>
    /// Название книги
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; init; } = null!;

    /// <summary>
    /// Голоса, отданные за книгу
    /// </summary>
    public uint Votes { get; init; }
}