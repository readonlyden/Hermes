using Hermes.Scheduling;

namespace ScheduledJobsExample;

public class LongRunningJob : IJob
{
    private readonly ILogger<SimpleJob> _logger;

    public LongRunningJob(ILogger<SimpleJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Long job started at: {time}", DateTimeOffset.Now);
        await Task.Delay(5000);
        _logger.LogInformation("Long job finished at: {time}", DateTimeOffset.Now);
    }
}
