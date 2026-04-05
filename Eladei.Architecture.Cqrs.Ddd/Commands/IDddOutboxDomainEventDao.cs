using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Служба сохранения доменных событий в outbox
/// </summary>
/// <remarks>Используется, если нужно сохранить доменные события в БД 
/// с последующей публикацией в отдельном процессе, например в job.
/// Сохраняйте события в той же транзакции, что и другие данные.
/// Для этого передавайте в метод SaveAsync ту же фабрику репозиториев, 
/// что используется в единице работы</remarks>
public interface IDddOutboxDomainEventDao {
    /// <summary>
    /// Сохранить событие предметной области в постоянное хранилище
    /// </summary>
    /// <param name="domainEvents">Событие предметной области</param>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}