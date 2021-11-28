using System;
using System.Threading;
using System.Threading.Tasks;
using Hermes.BackgroundTasks;
using Xunit;

namespace Hermes.Tests;

public class BackgroundTaskQueueTests
{
    private const int DefaultQueueCapacity = 10;

    [Fact]
    public async Task EnqueueDequeueShouldWorkAsExpected()
    {
        int numberOfCalls = 0;
        CancellationToken cancellationToken = default;

        var func = (CancellationToken _) =>
        {
            numberOfCalls++;
            return ValueTask.CompletedTask;
        };

        var backgroundQueue = new BackgroundTaskQueue(DefaultQueueCapacity);

        await backgroundQueue.EnqueueAsync(func);
        var funcFromQueue = await backgroundQueue.DequeueAsync(cancellationToken);

        await funcFromQueue(cancellationToken);

        Assert.Equal(1, numberOfCalls);
    }
}
