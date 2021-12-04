using Hermes.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hermes.Tests.BackgroundTasks;

public class ServiceCollectionTests
{
    [Fact]
    public void AddBackgroundTasks_ShouldAddServices_ToServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddBackgroundTasks();

        var provider = serviceCollection.BuildServiceProvider();

        Assert.Equal(2, serviceCollection.Count);
        Assert.NotNull(provider);
        Assert.NotNull(provider.GetService<IBackgroundTaskQueue>());
    }

    [Fact]
    public void AddBackgroundTasks_ShouldAddServices_ToServiceCollection_WithCustomCapacity()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddBackgroundTasks(options => options.QueueCapacity = 20);

        var provider = serviceCollection.BuildServiceProvider();

        Assert.Equal(2, serviceCollection.Count);
        Assert.NotNull(provider);
        Assert.NotNull(provider.GetService<IBackgroundTaskQueue>());

        var queue = provider.GetRequiredService<IBackgroundTaskQueue>()
            as BackgroundTaskQueue;

        Assert.NotNull(queue);
        Assert.Equal(20, queue!.Capacity);
    }
}

