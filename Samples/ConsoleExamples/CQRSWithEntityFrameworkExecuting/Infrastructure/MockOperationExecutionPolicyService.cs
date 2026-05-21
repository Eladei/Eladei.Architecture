using Eladei.Architecture.Cqrs;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Mock service for resolving operation execution policies
/// </summary>
public sealed class MockOperationExecutionPolicyService : IOperationExecutionPolicyService
{
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => new OperationExecutionPolicyBuilder().Build();
}