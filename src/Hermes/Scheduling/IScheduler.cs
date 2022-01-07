namespace Hermes.Scheduling;

public interface IScheduler
{
    Task AddJob<TJob>(JobOptions jobOptions, CancellationToken cancellationToken) where TJob : IJob;
    Task<IReadOnlyCollection<IJob>> GetJobsThatShouldRun(CancellationToken cancellationToken);
}
