using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.EntityFramework;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

public sealed class MockOperationExecutionPolicyProvider : IOperationExecutionPolicyProvider
{
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => new OperationExecutionPolicyBuilder().Build();
}
