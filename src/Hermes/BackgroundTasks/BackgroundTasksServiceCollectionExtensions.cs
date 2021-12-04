using Microsoft.Extensions.DependencyInjection;

namespace Hermes.BackgroundTasks;

public class BackgroundTasksConfig
{
    public const int DefaultQueueCapacity = 10;

    public int QueueCapacity { get; set; }
}

public static class BackgroundTasksServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTasks(
        this IServiceCollection services,
        Action<BackgroundTasksConfig>? optionsBuilder = null)
    {
        var backgroundTasksConfig = new BackgroundTasksConfig
        {
            QueueCapacity = BackgroundTasksConfig.DefaultQueueCapacity
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
