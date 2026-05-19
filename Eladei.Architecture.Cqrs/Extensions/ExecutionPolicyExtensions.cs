namespace Eladei.Architecture.Cqrs.Extensions;

/// <summary>
/// Extension methods for working with operation execution policies
/// </summary>
public static class ExecutionPolicyExtensions
{
    /// <summary>
    /// Adds an execution policy to the collection
    /// </summary>
    /// <typeparam name="T">The operation type</typeparam>
    /// <param name="dictionary">The dictionary of execution policies</param>
    /// <param name="policyFactory">The execution policy factory</param>
    public static void AddPolicy<T>(
        this Dictionary<Type, IOperationExecutionPolicy> dictionary,
        Func<IOperationExecutionPolicy> policyFactory)
        where T : IOperation
    {
        dictionary.Add(typeof(T), policyFactory());
    }
}