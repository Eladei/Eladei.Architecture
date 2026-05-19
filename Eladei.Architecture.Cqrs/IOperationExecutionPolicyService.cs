namespace Eladei.Architecture.Cqrs;

/// <summary>
/// Provides execution policies for operations
/// </summary>
public interface IOperationExecutionPolicyService
{
    /// <summary>
    /// Returns the execution policy for the specified operation
    /// </summary>
    /// <param name="operation">The operation</param>
    /// <returns>The operation execution policy</returns>
    IOperationExecutionPolicy GetExecutionPolicy(IOperation operation);
}