namespace Steward.Server.Data.Policies;

[Flags]
public enum WeekDays
{
    None      = 0,
    Sunday    = 1 << 0,
    Monday    = 1 << 1,
    Tuesday   = 1 << 2,
    Wednesday = 1 << 3,
    Thursday  = 1 << 4,
    Friday    = 1 << 5,
    Saturday  = 1 << 6,
}

public static class WeekDaysExtensions
{
    public static WeekDays ToWeekDays(this DayOfWeek day) =>
        day switch
        {
            DayOfWeek.Sunday => WeekDays.Sunday,
            DayOfWeek.Monday => WeekDays.Monday,
            DayOfWeek.Tuesday => WeekDays.Tuesday,
            DayOfWeek.Wednesday => WeekDays.Wednesday,
            DayOfWeek.Thursday => WeekDays.Thursday,
            DayOfWeek.Friday => WeekDays.Friday,
            DayOfWeek.Saturday => WeekDays.Saturday,
            _ => WeekDays.None
        };


    public static WeekDays ToWeekDays(
        this IEnumerable<DayOfWeek> days)
    {
        WeekDays result = WeekDays.None;

        foreach (var day in days)
        {
            result |= day.ToWeekDays();
        }

        return result;
    }


    public static bool Includes(
        this WeekDays days,
        DayOfWeek day)
    {
        return (days & day.ToWeekDays()) != 0;
    }


    public static IEnumerable<DayOfWeek> ToDayList(
        this WeekDays days)
    {
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            if (days.Includes(day))
            {
                yield return day;
            }
        }
    }
}