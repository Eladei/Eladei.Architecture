using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Команда для работы с Ddd
/// </summary>
/// <typeparam name="R">Тип возвращаемого результата</typeparam>
public abstract class DddCommandWithResultBase<R> : IDddCommand<R> {
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    public void ClearEvents() {
        _events.Clear();
    }

    public virtual Task BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    public abstract Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default);

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