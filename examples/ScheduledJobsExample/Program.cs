using Hermes.Scheduling;
using ScheduledJobsExample;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScheduler(scheduler =>
        {
            scheduler.AddJob<SimpleJob>(job =>
            {
                job.CronExpression = "*/5 * * * * *";
            });

            scheduler.AddJob<LongRunningJob>(job =>
            {
                job.CronExpression = "*/8 * * * * *";
            });
        });
    })
    .Build();

await host.RunAsync();
