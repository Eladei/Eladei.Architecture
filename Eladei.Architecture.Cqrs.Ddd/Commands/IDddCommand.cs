using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Команда
/// </summary>
public interface IDddCommand : ICommand {
    /// <summary>
    /// Доменные события
    /// </summary>
    IReadOnlyCollection<IDomainEvent> Events { get; }

    /// <summary>
    /// Очистка доменных событий
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Действия перед выполнением команды
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Необходимость выполнения команды</returns>
    Task<bool> BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения команды</returns>
    Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}

/// <summary>
/// Команда, возвращающая результат
/// </summary>
/// <typeparam name="R">Тип результата</typeparam>
public interface IDddCommand<R> : ICommand<R> {
    /// <summary>
    /// Доменные события
    /// </summary>
    IReadOnlyCollection<IDomainEvent> Events { get; }

    /// <summary>
    /// Очистка доменных событий
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Действия перед выполнением команды
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения операции</returns>
    Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}