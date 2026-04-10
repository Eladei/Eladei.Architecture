using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Команда
/// </summary>
/// <remarks>Интерфейс описывает команду, напрямую работающую с контекстом данных.
/// Такие команды реализуют transaction script</remarks>
/// <typeparam name="T">Тип контекста данных</typeparam>
public interface IEfCommand<T> : ICommand where T : DbContext
{
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
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Необходимость выполнения команды</returns>
    Task<bool> BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения команды</returns>
    Task ExecuteAsync(T context, CancellationToken cancellationToken);
}

/// <summary>
/// Команда, возвращающая результат
/// </summary>
/// <typeparam name="T">Тип контекста данных</typeparam>
/// <typeparam name="R">Тип результата</typeparam>
/// <remarks>Интерфейс описывает команду, напрямую работающую с контекстом данных.
/// Такие команды реализуют transaction script</remarks>
public interface IEfCommand<T, R> : ICommand<R> where T : DbContext
{
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
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения операции</returns>
    Task<R> ExecuteAsync(T context, CancellationToken cancellationToken);
}