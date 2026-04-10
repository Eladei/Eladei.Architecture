using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Исполнитель команд, работающий с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public interface IEfCommandExecutor<T> where T : DbContext
{
    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task ExecuteAsync(IEfCommand<T> command, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду и вернуть результат
    /// </summary>
    /// <typeparam name="R">Тип результата</typeparam>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task<R> ExecuteAsync<R>(IEfCommand<T, R> command, CancellationToken cancellationToken);
}