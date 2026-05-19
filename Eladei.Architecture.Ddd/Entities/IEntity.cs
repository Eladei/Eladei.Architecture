namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Entity
/// </summary>
/// <typeparam name="T">The type of the entity identifier</typeparam>
public interface IEntity<T>
{
    /// <summary>
    /// The entity identifier
    /// </summary>
    T Id { get; }
}