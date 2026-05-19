namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Command executor
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Executes a command
    /// </summary>
    /// <param name="command">The command</param>
    /// <param name="ct">The cancellation token</param>
    Task ExecuteAsync(ICommand command, CancellationToken ct);

    /// <summary>
    /// Executes a command
    /// </summary>
    /// <typeparam name="R">The result type</typeparam>
    /// <param name="command">The command</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The execution result</returns>
    Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct);
}