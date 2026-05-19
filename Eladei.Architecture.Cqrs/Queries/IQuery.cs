namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Query
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public interface IQuery<R> : IOperation { }