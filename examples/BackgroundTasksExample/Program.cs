using Hermes.BackgroundTasks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBackgroundTasks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", async ([FromServices] ILogger<Program> logger, [FromServices] IBackgroundTaskQueue backgroundTaskQueue) =>
{
    logger.LogInformation("Handling POST...");
    await backgroundTaskQueue.EnqueueAsync(async _ =>
    {
        logger.LogInformation("Handling background task...");

        await Task.Delay(5000);

        logger.LogInformation("Handled background task...");
    });
})
    .WithName("EndpointWithBackgroundTask");

app.Run();
