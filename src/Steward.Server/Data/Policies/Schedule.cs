namespace Steward.Server.Data.Policies;

public class Schedule
{
    public WeekDays Days { get; set; } = WeekDays.None;

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
}