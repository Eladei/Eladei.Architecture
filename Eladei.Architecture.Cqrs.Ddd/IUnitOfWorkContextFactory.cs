namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Фабрика контекста единицы работы
/// </summary>
public interface IUnitOfWorkContextFactory
{
    /// <summary>
    /// Создает контекст единицы работы
    /// </summary>
    /// <returns></returns>
    IUnitOfWorkContext CreateContext();
}