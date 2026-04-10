using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// Информация о книге в рейтинге
/// </summary>
public sealed class BookInRating : Aggregate<Guid>
{
    /// <summary>
    /// Создает объект класса BookInRating
    /// </summary>
    /// <param name="id">Идентификатор книги</param>
    /// <param name="name">Название книги</param>
    /// <param name="author">Автор</param>
    /// <param name="votes">Число голосов, отданных за книгу</param>
    /// <exception cref="ArgumentException"></exception>
    public BookInRating(Guid id, string name, string author, uint votes = 0) : base(id)
    {
        Name = name ?? throw new ArgumentException("Не указано название книги");
        Author = author ?? throw new ArgumentException("Не указан автор книги");
        Votes = votes;
    }

    /// <summary>
    /// Название книги
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Автор
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Голоса, отданные за книгу
    /// </summary>
    public uint Votes { get; private set; }

    /// <summary>
    /// Проголосовать за книгу
    /// </summary>
    public void Vote()
    {
        Votes++;

        AddDomainEvent(new BookWasVotedDomainEvent(Id));
    }
}