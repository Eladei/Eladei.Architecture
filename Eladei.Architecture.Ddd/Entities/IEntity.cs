namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Сущность
/// </summary>
/// <typeparam name="T">Тип идентификатора сущности</typeparam>
public interface IEntity<T> {
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    T Id { get; }
}