namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Command that does not return a result
/// </summary>
public interface ICommand : IOperation { }

/// <summary>
/// Command that returns a result
/// </summary>
/// <typeparam name="R">The result type</typeparam>
public interface ICommand<R> : ICommand { }