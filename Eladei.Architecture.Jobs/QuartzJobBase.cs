using Eladei.Architecture.Jobs.Quartz.Properties;
using Eladei.Architecture.Logging;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Eladei.Architecture.Jobs.Quartz;

/// <summary>
/// Base Quartz job
/// </summary>
public abstract class QuartzJobBase : IJob
{
    private readonly string _jobName;

    protected readonly ICorrelationContext _correlationContext;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Creates an instance of the Quartz job
    /// </summary>
    /// <param name="correlationContext">The correlation context used for tracing job execution</param>
    /// <param name="logger">Optional logger instance</param>
    public QuartzJobBase(ICorrelationContext correlationContext, ILogger? logger = null)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;

        _jobName = GetType().Name;
    }
    
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        using (_correlationContext.SetCorrelationId(Guid.NewGuid()))
        {
            try
            {
                LogJobStarted();

                await Perform(context.CancellationToken);

                LogJobFinished();
            }
            catch (OperationCanceledException ex)
            {
                LogJobCancelled(ex);

                throw;
            }
            catch (Exception ex)
            {
                LogJobError(ex);
            }
        }
    }

    #region Logging methods

    /// <summary>
    /// Logs the start of job execution
    /// </summary>
    protected virtual void LogJobStarted()
    {
        var msg = string.Format(Resources.JobStarted, _jobName);
        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs successful job completion
    /// </summary>
    protected virtual void LogJobFinished()
    {
        var msg = string.Format(Resources.JobFinished, _jobName);
        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs job cancellation
    /// </summary>
    /// <param name="ex">The cancellation exception</param>
    protected virtual void LogJobCancelled(OperationCanceledException ex)
    {
        var msg = string.Format(Resources.JobCancelled, _jobName);
        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Logs a job execution error
    /// </summary>
    /// <param name="ex">The exception that occurred during execution</param>
    protected virtual void LogJobError(Exception ex)
    {
        var errorMsg = string.Format(Resources.JobError, _jobName);
        _logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Executes the main job logic
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    protected abstract Task Perform(CancellationToken cancellationToken);
}