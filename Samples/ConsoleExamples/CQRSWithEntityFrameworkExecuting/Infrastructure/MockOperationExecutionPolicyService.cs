using Eladei.Architecture.Cqrs;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Мок службы запроса политики выполнения операции
/// </summary>
public sealed class MockOperationExecutionPolicyService : IOperationExecutionPolicyService {
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => new OperationExecutionPolicyBuilder().Build();
}