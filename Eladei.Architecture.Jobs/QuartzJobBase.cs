using Eladei.Architecture.Jobs.Quartz.Properties;
using Eladei.Architecture.Logging;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Eladei.Architecture.Jobs.Quartz;

/// <summary>
/// Базовый Job для Quartz
/// </summary>
public abstract class QuartzJobBase : IJob {
    private readonly string _jobName;

    protected readonly ICorrelationContext _correlationContext;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Создает объект класса QuartzJobBase
    /// </summary>
    /// <param name="logger">Тип базового job</param>
    public QuartzJobBase(ICorrelationContext correlationContext, ILogger? logger = null) {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;

        _jobName = GetType().Name;
    }

    public async Task Execute(IJobExecutionContext context) {
        using (_correlationContext.SetCorrelationId(Guid.NewGuid())) {
            try {
                LogJobStarted();

                await Perform(context.CancellationToken);

                LogJobFinished();
            }
            catch (OperationCanceledException ex) {
                LogJobCancelled(ex);

                throw;
            }
            catch (Exception ex) {
                LogJobError(ex);
            }
        }
    }

    #region Методы логирования

    /// <summary>
    /// Логировать начало работы job
    /// </summary>
    protected virtual void LogJobStarted() {
        var msg = string.Format(Resources.JobStarted, _jobName);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логировать завершение работы job
    /// </summary>
    protected virtual void LogJobFinished() {
        var msg = string.Format(Resources.JobFinished, _jobName);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логировать отмены работы job
    /// </summary>
    /// <param name="ex">Данные по отмене операции</param>
    protected virtual void LogJobCancelled(OperationCanceledException ex) {
        var msg = string.Format(Resources.JobCancelled, _jobName);

        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Логировать ошибку в процессе выполнения job
    /// </summary>
    /// <param name="ex">Ошибка выполнения job</param>
    protected virtual void LogJobError(Exception ex) {
        var errorMsg = string.Format(Resources.JobError, _jobName);

        _logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Выполнить основные операции job
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    protected abstract Task Perform(CancellationToken cancellationToken);
}