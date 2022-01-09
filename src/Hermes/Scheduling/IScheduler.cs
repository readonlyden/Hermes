namespace Hermes.Scheduling;

public interface IScheduler
{
    Task<IReadOnlyCollection<IJob>> GetJobsThatShouldRun(CancellationToken cancellationToken);
}
