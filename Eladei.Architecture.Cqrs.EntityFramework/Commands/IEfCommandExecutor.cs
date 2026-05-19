using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Command executor for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
public interface IEfCommandExecutor<T> where T : DbContext
{
    /// <summary>
    /// Executes a command
    /// </summary>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(IEfCommand<T> command, CancellationToken cancellationToken);

    /// <summary>
    /// Executes a command and returns a result
    /// </summary>
    /// <typeparam name="R">The result type</typeparam>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<R> ExecuteAsync<R>(IEfCommand<T, R> command, CancellationToken cancellationToken);
}