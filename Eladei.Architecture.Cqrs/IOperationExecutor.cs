using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Operation executor
/// </summary>
public interface IOperationExecutor : ICommandExecutor, IQueryExecutor { }