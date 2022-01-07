using System.Collections.Concurrent;
using Cronos;
using Microsoft.Extensions.DependencyInjection;

namespace Hermes.Scheduling;

public class Scheduler : IScheduler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, IJobDefinition> _jobDefinitions = new();

    public Scheduler(IServiceProvider serviceProvider, IJobCollection jobDefinitions)
    {
        _serviceProvider = serviceProvider;

        foreach (var definition in jobDefinitions)
        {
            _jobDefinitions.TryAdd(definition.Name, definition);
        }
    }

    public Task AddJob<TJob>(JobOptions jobOptions, CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        var jobDefinition = CreateJobDefinition<TJob>(jobOptions);

        _jobDefinitions.AddOrUpdate(jobDefinition.Name, jobDefinition, (name, job) => jobDefinition);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<IJob>> GetJobsThatShouldRun(CancellationToken cancellationToken = default)
    {
        var currentTime = DateTimeOffset.UtcNow;

        var jobDefinitionsThatShouldRun = _jobDefinitions.Values
            .Where(t => t.ShouldRun(currentTime))
            .ToList();

        using var scope = _serviceProvider.CreateScope();

        var jobsToRun = jobDefinitionsThatShouldRun.Select(jobDefinition =>
            {
                return (IJob)scope.ServiceProvider.GetRequiredService(jobDefinition.JobType);
            })
            .ToList();

        foreach (var jobDefinition in jobDefinitionsThatShouldRun)
        {
            jobDefinition.ScheduleNextRun();
        }

        return Task.FromResult<IReadOnlyCollection<IJob>>(jobsToRun);
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
