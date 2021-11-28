using Microsoft.Extensions.DependencyInjection;

namespace Hermes.BackgroundTasks;

public static class BackgroundTasksServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<BackgroundTasksHostedService>();

        return services;
    }
}
