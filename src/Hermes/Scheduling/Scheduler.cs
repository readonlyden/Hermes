using System.Collections.Concurrent;
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
}
