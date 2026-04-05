namespace Eladei.Architecture.Cqrs.Extensions;

/// <summary>
/// Методы расширения для работы с политиками выполнения операций
/// </summary>
public static class ExecutionPolicyExtensions {
    /// <summary>
    /// Добавляет политику выполнения операций в коллекцию
    /// </summary>
    /// <typeparam name="T">Тип операции</typeparam>
    /// <param name="dictionary">Словарь политик выполнения операций</param>
    /// <param name="policyFactory">Фабрика политики выполнения операции</param>
    public static void AddPolicy<T>(this Dictionary<Type, IOperationExecutionPolicy> dictionary, Func<IOperationExecutionPolicy> policyFactory) where T : IOperation {
        dictionary.Add(typeof(T), policyFactory());
    }
}