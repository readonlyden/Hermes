namespace Hermes.Scheduling;

public class JobOptions
{
    public string CronExpression { get; set; } = string.Empty;
    public bool RunImmediately { get; set; } = false;
    public string JobName { get; set; } = string.Empty;
}
