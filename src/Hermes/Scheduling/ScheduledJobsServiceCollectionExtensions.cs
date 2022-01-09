using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hermes.Scheduling;

public class SchedulerOptionsBuilder
{
    private readonly IJobCollection _jobCollection;
    private readonly IServiceCollection _services;

    public SchedulerOptionsBuilder(IJobCollection jobCollection, IServiceCollection services)
    {
        _jobCollection = jobCollection;
        _services = services;
    }

    /// <summary>
    /// Registers job in the scheduler, also add it to IServiceCollection as a scoped service
    /// </summary>
    /// <typeparam name="TJob">Job type</typeparam>
    /// <param name="optionsBuilder">Job options</param>
    /// <returns></returns>
    public SchedulerOptionsBuilder AddJob<TJob>(Action<JobOptions> optionsBuilder)
        where TJob : class, IJob
    {
        var jobType = typeof(TJob);
        var options = new JobOptions();

        optionsBuilder(options);

        if (string.IsNullOrEmpty(options.JobName))
        {
            options.JobName = jobType.Name;
        }

        _services.AddScoped(jobType);
        var jobDefinition = CreateJobDefinition<TJob>(options);
        _jobCollection.Add(jobDefinition);

        return this;
    }

    private IJobDefinition CreateJobDefinition<TJob>(JobOptions options)
        where TJob : IJob
    {
        var currentTime = DateTimeOffset.UtcNow;
        var timeZone = TimeZoneInfo.Local;

        CronExpression crontabSchedule;

        if (options.CronExpression.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length == 6)
        {
            crontabSchedule = CronExpression.Parse(options.CronExpression, CronFormat.IncludeSeconds);
        }
        else
        {
            crontabSchedule = CronExpression.Parse(options.CronExpression, CronFormat.Standard);
        }

        var nextRunTime = options.RunImmediately ? currentTime : crontabSchedule.GetNextOccurrence(currentTime, timeZone)!.Value;

        return new JobDefinition(options.JobName, typeof(TJob), crontabSchedule, nextRunTime, timeZone);
    }
}

public static class ScheduledJobsServiceCollectionExtensions
{ 
    public static IServiceCollection AddScheduler(
        this IServiceCollection services, Action<SchedulerOptionsBuilder> schedulerOptionsConfigurator)
    {
        var jobCollection = new JobCollection();
        services.AddSingleton<IJobCollection>(jobCollection);

        var schedulerOptionsBuilder = new SchedulerOptionsBuilder(jobCollection, services);

        schedulerOptionsConfigurator(schedulerOptionsBuilder);

        services.AddSingleton<IScheduler, Scheduler>();
        services.AddHostedService<ScheduledJobsHostedService>();

        return services;
    }

    public static IServiceCollection AddScheduler(
        this IServiceCollection services)
    {
        services.AddSingleton<IScheduler, Scheduler>();
        services.AddHostedService<ScheduledJobsHostedService>();

        return services;
    }
}
