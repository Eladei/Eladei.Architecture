using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command
/// </summary>
public interface IDddCommand : ICommand
{
    /// <summary>
    /// Integration events
    /// </summary>
    IReadOnlyCollection<IIntegrationEvent> Events { get; }

    /// <summary>
    /// Clears integration events
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Actions before command execution
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Whether the command should be executed</returns>
    Task<bool> BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}

/// <summary>
/// Command that returns a result
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public interface IDddCommand<R> : ICommand<R>
{
    /// <summary>
    /// Integration events
    /// </summary>
    IReadOnlyCollection<IIntegrationEvent> Events { get; }

    /// <summary>
    /// Clears integration events
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Actions before command execution
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task BeforeExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The execution result</returns>
    Task<R> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);
}
