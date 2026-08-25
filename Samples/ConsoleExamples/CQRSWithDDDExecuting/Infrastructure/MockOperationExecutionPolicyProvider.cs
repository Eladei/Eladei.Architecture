using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.Ddd;

namespace CqrsWithDddExecuting.Infrastructure;

public sealed class MockOperationExecutionPolicyProvider : IOperationExecutionPolicyProvider
{
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => new OperationExecutionPolicyBuilder().Build();
}
