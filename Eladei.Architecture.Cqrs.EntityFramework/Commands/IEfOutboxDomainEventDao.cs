using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Служба сохранения доменных событий в outbox
/// </summary>
/// <typeparam name="T">Тип контекста данных</typeparam>
/// <remarks>Используется, если нужно сохранить доменные события в БД 
/// с последующей публикацией в отдельном процессе, например в job.
/// Сохраняйте события в той же транзакции, что и другие данные.
/// Для этого передавайте в метод SaveAsync тот же контекст данных</remarks>
public interface IEfOutboxDomainEventDao<T> where T : DbContext {
    /// <summary>
    /// Сохранить доменное событие в постоянное хранилище
    /// </summary>
    /// <param name="domainEvents">Доменные события</param>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, T context, CancellationToken cancellationToken);
}