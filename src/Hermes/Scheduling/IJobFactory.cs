namespace Hermes.Scheduling;

public interface IJobFactory
{
    TJob Create<TJob>() where TJob : IJob;
}
