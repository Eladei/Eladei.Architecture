using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Команда, возвращающая результат
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
/// <typeparam name="R">Тип результата</typeparam>
/// <remarks>Команда напрямую работает с контекстом данных, реализует transaction script</remarks>
public abstract class EfCommandWithResultBase<T, R> : IEfCommand<T, R> where T : DbContext {
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// События предметной области
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    public void ClearEvents() {
        _events.Clear();
    }

    public virtual Task BeforeExecuteAsync(T context, CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    public abstract Task<R> ExecuteAsync(T context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить доменные события
    /// </summary>
    /// <param name="domainEvents">Доменные события</param>
    /// <remarks>Добавленные доменные события доступны через коллекцию Events.
    /// Используются для возможности сохранения событий в outbox обработчиком команд</remarks>
    protected void SaveDomainEvents(params IDomainEvent[] domainEvents) {
        _events.AddRange(domainEvents);
    }
}