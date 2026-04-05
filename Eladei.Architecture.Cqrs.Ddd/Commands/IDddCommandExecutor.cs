namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Исполнитель команд
/// </summary>
public interface IDddCommandExecutor {
    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task ExecuteAsync(IDddCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Выполнить команду и вернуть результат
    /// </summary>
    /// <typeparam name="R">Тип результата</typeparam>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task<R> ExecuteAsync<R>(IDddCommand<R> command, CancellationToken cancellationToken);
}