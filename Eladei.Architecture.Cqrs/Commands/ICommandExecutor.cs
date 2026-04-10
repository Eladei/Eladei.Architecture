namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Исполнитель команд
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="command">Команда</param>
    /// <param name="ct">Токен отмены операции</param>
    Task ExecuteAsync(ICommand command, CancellationToken ct);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <typeparam name="R">Тип результата</typeparam>
    /// <param name="command">Команда</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат выполнения команды</returns>
    Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct);
}