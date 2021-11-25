namespace Hermes.Scheduling;

public interface IJob
{
    Task ExecuteAsync(CancellationToken token = default);
}
