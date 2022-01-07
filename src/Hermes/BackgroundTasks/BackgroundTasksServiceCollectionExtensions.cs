using Microsoft.Extensions.DependencyInjection;

namespace Hermes.BackgroundTasks;

public class BackgroundTasksOptions
{
    public const int DefaultQueueCapacity = 10;

    public int QueueCapacity { get; set; }
}

public static class BackgroundTasksServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTasks(
        this IServiceCollection services,
        Action<BackgroundTasksOptions>? optionsBuilder = null)
    {
        var backgroundTasksConfig = new BackgroundTasksOptions
        {
            QueueCapacity = BackgroundTasksOptions.DefaultQueueCapacity
        };

        if (optionsBuilder != null)
        {
            optionsBuilder(backgroundTasksConfig);
        }


        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>(provider =>
        {
            return new BackgroundTaskQueue(backgroundTasksConfig.QueueCapacity);
        });

        services.AddHostedService<BackgroundTasksHostedService>();

        return services;
    }
}
