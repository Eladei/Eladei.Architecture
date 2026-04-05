namespace Eladei.Architecture.Cqrs.Ddd; 

/// <summary>
/// Контекст единицы работы
/// </summary>
public interface IUnitOfWorkContext : IUnitOfWork, IRepositoryFactory { }