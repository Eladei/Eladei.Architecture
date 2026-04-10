using Quartz;

namespace Eladei.Architecture.Jobs.Quartz.Extensions;

/// <summary>
/// Расширение для Quartz
/// </summary>
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    /// <summary>
    /// Добавляет новую Job
    /// </summary>
    /// <typeparam name="T">Тип Job</typeparam>
    /// <param name="q">Конфигуратор</param>
    /// <param name="cron">Периодичность запуска Job в формате CRON</param>
    public static void AddNewJob<T>(this IServiceCollectionQuartzConfigurator q, string cron) where T : IJob
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