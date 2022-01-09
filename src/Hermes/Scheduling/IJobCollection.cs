namespace Hermes.Scheduling;

public interface IJobCollection : IList<IJobDefinition>
{
}

public class JobCollection : List<IJobDefinition>, IJobCollection
{
}
