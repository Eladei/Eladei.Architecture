using Eladei.Architecture.Cqrs;
using Eladei.Architecture.Cqrs.Extensions;
using Eladei.BookInfo.Domain.Commands;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Eladei.BookInfo.Api.Policies;

/// <summary>
/// Operation execution policy service
/// </summary>
public sealed class OperationExecutionPolicyService : IOperationExecutionPolicyService
{
    static OperationExecutionPolicyService()
    {
        var policies = new Dictionary<Type, IOperationExecutionPolicy>();

        policies.AddPolicy<AddBookCommand>(()
            => new OperationExecutionPolicyBuilder()
                .MaxAttemptsCount(3)
                .RetryOn(typeof(DbUpdateConcurrencyException))
                .Build());

        _policies = policies.AsReadOnly();
    }

    private static ReadOnlyDictionary<Type, IOperationExecutionPolicy> _policies;

    private static readonly IOperationExecutionPolicy _defaultPolicy = new OperationExecutionPolicyBuilder().Build();

    /// <inheritdoc />
    public IOperationExecutionPolicy GetExecutionPolicy(IOperation operation)
        => _policies.TryGetValue(operation.GetType(), out var policy)
            ? policy
            : _defaultPolicy;
}