using Quartz;

namespace Eladei.Architecture.Jobs.Quartz.Extensions;

/// <summary>
/// Quartz configuration extensions
/// </summary>
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    /// <summary>
    /// Adds a new scheduled job
    /// </summary>
    /// <typeparam name="T">The job type</typeparam>
    /// <param name="q">The Quartz configurator</param>
    /// <param name="cron">The CRON expression defining the job schedule</param>
    public static void AddNewJob<T>(this IServiceCollectionQuartzConfigurator q, string cron)
        where T : IJob
    {
        var jobName = typeof(T).Name;
        var jobKey = new JobKey(jobName);

        q.AddJob<T>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{jobName}-trigger")
            .WithCronSchedule(cron)
        );
    }
}