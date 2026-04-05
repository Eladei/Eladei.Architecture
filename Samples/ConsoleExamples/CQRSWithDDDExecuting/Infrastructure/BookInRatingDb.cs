namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Информация о книге в рейтинге
/// </summary>
public sealed class BookInRatingDb {
    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название книги
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; set; } = null!;

    /// <summary>
    /// Голоса, отданные за книгу
    /// </summary>
    public uint Votes { get; set; }
}