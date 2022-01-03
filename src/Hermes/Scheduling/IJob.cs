namespace Hermes.Scheduling;

public interface IJob
{
    ValueTask ExecuteAsync(CancellationToken token = default);
}
