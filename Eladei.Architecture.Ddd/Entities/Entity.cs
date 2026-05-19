namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Entity base class
/// </summary>
/// <typeparam name="T">The type of the entity identifier</typeparam>
public abstract class Entity<T> : IEntity<T>
{
    /// <summary>
    /// Creates an instance of the entity
    /// </summary>
    /// <param name="id">The entity identifier</param>
    public Entity(T id)
    {
        Id = id;
    }

    /// <summary>
    /// The entity identifier
    /// </summary>
    public T Id { get; }
}