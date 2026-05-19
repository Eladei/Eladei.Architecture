using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Command for working with Entity Framework
/// </summary>
/// <remarks>
/// The interface defines a command that works directly with the database context.
/// Such commands implement the transaction script pattern.
/// </remarks>
/// <typeparam name="T">The database context type</typeparam>
public interface IEfCommand<T> : ICommand where T : DbContext
{
    /// <summary>
    /// Domain events
    /// </summary>
    IReadOnlyCollection<IDomainEvent> Events { get; }

    /// <summary>
    /// Clears domain events
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Actions executed before the command execution
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Indicates whether the command should be executed</returns>
    Task<bool> BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(T context, CancellationToken cancellationToken);
}

/// <summary>
/// Command that returns a result
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <typeparam name="R">The result type</typeparam>
/// <remarks>
/// The interface defines a command that works directly with the database context.
/// Such commands implement the transaction script pattern.
/// </remarks>
public interface IEfCommand<T, R> : ICommand<R> where T : DbContext
{
    /// <summary>
    /// Domain events
    /// </summary>
    IReadOnlyCollection<IDomainEvent> Events { get; }

    /// <summary>
    /// Clears domain events
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// Actions executed before the command execution
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the operation</returns>
    Task<R> ExecuteAsync(T context, CancellationToken cancellationToken);
}