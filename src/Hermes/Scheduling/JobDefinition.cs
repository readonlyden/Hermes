using Cronos;

namespace Hermes.Scheduling;

public class JobDefinition : IJobDefinition
{
    public string Name { get; set; }

    public CronExpression Schedule { get; set; }

    public Type JobType { get; set; }

    public DateTimeOffset LastRunTime { get; set; }

    public DateTimeOffset NextRunTime { get; set; }

    public TimeZoneInfo TimeZoneInfo { get; set; }

    public JobDefinition(string name, Type jobType, CronExpression cronExpression, DateTimeOffset nextRunTime, TimeZoneInfo timeZoneInfo)
    {
        Name = name;
        JobType = jobType;
        Schedule = cronExpression;
        NextRunTime = nextRunTime;
        TimeZoneInfo = timeZoneInfo;
    }

    public void ScheduleNextRun()
    {
        LastRunTime = NextRunTime;
        NextRunTime = Schedule.GetNextOccurrence(NextRunTime, TimeZoneInfo)!.Value;
    }

    public bool ShouldRun(DateTimeOffset currentTime)
    {
        return NextRunTime < currentTime && LastRunTime != NextRunTime;
    }
}
