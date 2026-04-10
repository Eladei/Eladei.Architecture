namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Единица работы
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Начать транзакцию
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Подтвердить транзакцию
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Откатить транзакцию
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}