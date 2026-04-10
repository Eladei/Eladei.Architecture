namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Сущность
/// </summary>
/// <typeparam name="T">Тип идентификатора сущности</typeparam>
public abstract class Entity<T> : IEntity<T>
{
    /// <summary>
    /// Создает объект класса Entity
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    public Entity(T id)
    {
        Id = id;
    }

    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    public T Id { get; }
}