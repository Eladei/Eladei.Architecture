using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Команда
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
/// <remarks>Команда напрямую работает с контекстом данных, реализует transaction script</remarks>
public abstract class EfCommandBase<T> : IEfCommand<T> where T : DbContext
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    /// События предметной области
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> Events => _events;

    public void ClearEvents()
    {
        _events.Clear();
    }

    public virtual Task<bool> BeforeExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public abstract Task ExecuteAsync(T context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить доменные события
    /// </summary>
    /// <param name="domainEvents">Доменные события</param>
    /// <remarks>Добавленные доменные события доступны через коллекцию Events.
    /// Используются для возможности сохранения событий в outbox обработчиком команд</remarks>
    protected void SaveDomainEvents(params IDomainEvent[] domainEvents)
    {
        _events.AddRange(domainEvents);
    }
}