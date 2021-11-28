namespace Hermes.Scheduling;

// TODO: Implement Scheduling
public interface IJobFactory
{
    TJob Create<TJob>() where TJob : IJob;
}
