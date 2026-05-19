namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command executor
/// </summary>
public interface IDddCommandExecutor
{
    /// <summary>
    /// Executes a command
    /// </summary>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(IDddCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Executes a command and returns a result
    /// </summary>
    /// <typeparam name="R">The result type</typeparam>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<R> ExecuteAsync<R>(IDddCommand<R> command, CancellationToken cancellationToken);
}