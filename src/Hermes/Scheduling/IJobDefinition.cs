using Cronos;

namespace Hermes.Scheduling;

public interface IJobDefinition
{
    string Name { get; }
    Type JobType { get; }

    DateTimeOffset LastRunTime { get; set; }
    DateTimeOffset NextRunTime { get; set; }
    CronExpression Schedule { get; }
    TimeZoneInfo TimeZoneInfo { get; }

    void ScheduleNextRun();
    bool ShouldRun(DateTimeOffset currentTime);
}
