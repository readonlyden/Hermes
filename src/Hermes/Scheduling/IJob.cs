namespace Hermes.Scheduling;

// TODO: Implement Scheduling
public interface IJob
{
    Task ExecuteAsync(CancellationToken token = default);
}
