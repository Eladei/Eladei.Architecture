using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Исполнитель операций, работающих с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public class OperationExecutor : IOperationExecutor
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IQueryExecutor _queryExecutor;

    /// <summary>
    /// Создает объект класса EfOperationExecutor
    /// </summary>
    /// <param name="commandExecutor">Исполнитель команды</param>
    /// <param name="queryExecutor">Исполнитель запроса</param>
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

    public Task ExecuteAsync(ICommand command, CancellationToken ct)
        => _commandExecutor.ExecuteAsync(command, ct);

    public Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct)
        => _commandExecutor.ExecuteAsync(command, ct);

    public Task<R> ExecuteAsync<R>(IQuery<R> query, CancellationToken ct)
        => _queryExecutor.ExecuteAsync(query, ct);
}