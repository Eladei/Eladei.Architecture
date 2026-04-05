namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Команда, возвращающая результат
/// </summary>
public interface ICommand : IOperation { }

/// <summary>
/// Команда, возвращающая результат
/// </summary>
/// <typeparam name="R">Тип результата</typeparam>
public interface ICommand<R> : ICommand { }