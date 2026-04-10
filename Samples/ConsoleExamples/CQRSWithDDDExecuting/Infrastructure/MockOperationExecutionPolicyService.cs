using Eladei.Architecture.Cqrs;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Мок службы запроса политики выполниния операции
/// </summary>
public sealed class MockOperationExecutionPolicyService : IOperationExecutionPolicyService
{
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => new OperationExecutionPolicyBuilder().Build();
}