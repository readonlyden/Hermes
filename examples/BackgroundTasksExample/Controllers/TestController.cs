using Hermes.BackgroundTasks;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundTasksExample.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController: ControllerBase
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IBackgroundTaskQueue backgroundTaskQueue,
        ILogger<TestController> logger
    )
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> DoSomething()
    {
        string requestId = HttpContext.TraceIdentifier;

        _logger.LogInformation("Handling request {Id}", requestId);

        await _backgroundTaskQueue.EnqueueAsync(async cancellationToken =>
        {
            _logger.LogInformation("Handling background task for request {Id}", requestId);

            await Task.Delay(5000, cancellationToken);

            _logger.LogInformation("Handled background task for request {Id}", requestId);
        });

        _logger.LogInformation("Handled request {Id}", requestId);

        return Ok();
    }
}
