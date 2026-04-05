using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Базовый класс команды
/// </summary>
public abstract class DddCommandBase : IDddCommand {
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyCollection<IDomainEvent> Events => _events;

    public void ClearEvents() { 
        _events.Clear();
    }

    public virtual Task<bool> BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default) {
        return Task.FromResult(true);
    }

    public abstract Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить доменные события
    /// </summary>
    /// <param name="domainEvents">Доменные события</param>
    /// <remarks>Добавленные доменные события доступны через коллекцию Events.
    /// Используются для возможности сохранения событий в outbox обработчиком команд</remarks>
    protected void AddDomainEvents(params IDomainEvent[] domainEvents) {
        _events.AddRange(domainEvents);
    }
}