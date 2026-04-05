using Eladei.Architecture.Ddd.Repositories;

namespace Eladei.Architecture.Cqrs.Ddd; 

/// <summary>
/// Фабрика репозиториев
/// </summary>
/// <remarks>Учитывайте, что создаваемый репозиторий 
/// должен работать с тем же контекстом, что и единица работы. 
/// Это необходимо для сохранения изменений единицей работы в одной транзакции</remarks>
public interface IRepositoryFactory {
    /// <summary>
    /// Создает репозиторий
    /// </summary>
    /// <typeparam name="R">Тип репозитория</typeparam>
    /// <returns>Репозиторий</returns>
    R CreateRepository<R>() where R : IRepository;
}