using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Operation executor for Entity Framework operations
/// </summary>
/// <typeparam name="T">The data context type</typeparam>
public class OperationExecutor : IOperationExecutor
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    /// <summary>
    /// Creates a new instance of <see cref="OperationExecutor"/>
    /// </summary>
    /// <param name="commandExecutor">The command executor</param>
    /// <param name="queryExecutor">The query executor</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OperationExecutor(
        ICommandExecutor commandExecutor,
        IQueryExecutor queryExecutor)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _queryExecutor = queryExecutor
            ?? throw new ArgumentNullException(nameof(queryExecutor));
    }

    /// <inheritdoc />
    public Task ExecuteAsync(ICommand command, CancellationToken ct)
        => _commandExecutor.ExecuteAsync(command, ct);

    /// <inheritdoc />
    public Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct)
        => _commandExecutor.ExecuteAsync(command, ct);

    /// <inheritdoc />
    public Task<R> ExecuteAsync<R>(IQuery<R> query, CancellationToken ct)
        => _queryExecutor.ExecuteAsync(query, ct);
}