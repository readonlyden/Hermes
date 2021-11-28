using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hermes.BackgroundTasks;

public class BackgroundTasksHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<BackgroundTasksHostedService> _logger;

    public BackgroundTasksHostedService(IBackgroundTaskQueue taskQueue,
        ILogger<BackgroundTasksHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundTasks Hosted Service is running");

        await ProcessBackgroundTasks(stoppingToken);
    }

    private async Task ProcessBackgroundTasks(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgrounTasks Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
