namespace Steward.Server.Data.Policies;

public class Allowance
{
    public int? DailyTimeMinutes { get; set; }

    public int? MaxSessionMinutes { get; set; }

    public int? DailyUnlocks { get; set; }
}