using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hermes.Scheduling;

public class ScheduledJobsHostedService : BackgroundService
{
    private readonly IScheduler _scheduler;
    private readonly ILogger _logger;

    public ScheduledJobsHostedService(IScheduler scheduler,
        ILogger<ScheduledJobsHostedService> logger)
    {
        _scheduler = scheduler;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCurrentJobs(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task RunCurrentJobs(CancellationToken stoppingToken)
    {
        var currentTime = DateTimeOffset.UtcNow;

        var jobsThatShouldRun = await _scheduler.GetJobsThatShouldRun(stoppingToken);

        if (!jobsThatShouldRun.Any())
        {
            return;
        }

        try
        {
            foreach(var job in jobsThatShouldRun)
            {
                var runTask = () => job.ExecuteAsync(stoppingToken).ConfigureAwait(false);
                await Task.Run(runTask, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while running the job");
        }       
    }
}
