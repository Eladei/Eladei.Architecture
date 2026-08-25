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

    /// <summary>
    /// The correlation context used for tracing job execution
    /// </summary>
    protected readonly ICorrelationContext CorrelationContext;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger? Logger;

    /// <summary>
    /// Creates an instance of the Quartz job
    /// </summary>
    /// <param name="correlationContext">The correlation context used for tracing job execution</param>
    /// <param name="logger">The logger</param>
    public QuartzJobBase(ICorrelationContext correlationContext, ILogger? logger = null)
    {
        CorrelationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        Logger = logger;

        _jobName = GetType().Name;
    }

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        using (CorrelationContext.SetCorrelationId(Guid.NewGuid()))
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
        Logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs successful job completion
    /// </summary>
    protected virtual void LogJobFinished()
    {
        var msg = string.Format(Resources.JobFinished, _jobName);
        Logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs job cancellation
    /// </summary>
    /// <param name="ex">The cancellation exception</param>
    protected virtual void LogJobCancelled(OperationCanceledException ex)
    {
        var msg = string.Format(Resources.JobCancelled, _jobName);
        Logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Logs a job execution error
    /// </summary>
    /// <param name="ex">The exception that occurred during execution</param>
    protected virtual void LogJobError(Exception ex)
    {
        var errorMsg = string.Format(Resources.JobError, _jobName);
        Logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Executes the main job logic
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    protected abstract Task Perform(CancellationToken cancellationToken);
}
