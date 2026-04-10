namespace Eladei.Architecture.Logging;

/// <summary>
/// Контекст корреляции
/// </summary>
/// <remarks>Необходим для проброса correlationId при логировании
/// для отслеживания всей цепочки операций</remarks>
public interface ICorrelationContext
{
    /// <summary>
    /// Id для сквозного отслеживания
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Установить Id для сквозного отслеживания
    /// </summary>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    /// <returns></returns>
    IDisposable SetCorrelationId(Guid correlationId);
}