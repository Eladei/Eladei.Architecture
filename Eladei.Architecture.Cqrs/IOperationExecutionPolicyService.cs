namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Служба запроса политики выполнения операции
/// </summary>
public interface IOperationExecutionPolicyService
{
    /// <summary>
    /// Возвращает политику выполнения операции
    /// </summary>
    /// <param name="operation">Операция</param>
    /// <returns>Политика выполнения команды</returns>
    IOperationExecutionPolicy GetExecutionPolicy(IOperation operation);
}