namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Запрос
/// </summary>
/// <typeparam name="R">Тип результата</typeparam>
public interface IQuery<R> : IOperation { }