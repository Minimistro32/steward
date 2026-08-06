namespace Steward.Server.Application;

public sealed class PolicyEvaluation
{
    public required bool IsScheduled { get; init; }

    // ACTIONABLE
    public required AccessState State { get; init; }
    public required int? MaxRequestMinutes { get; init; }

    // EXPLANATORY
    public required DateTimeOffset? ScheduleEndsAt { get; init; }
    public required int? EffectiveMinutesRemaining { get; init; }
    public required int? DailyMinutesRemaining { get; init; }
    public required int? UnlocksRemaining { get; init; }

}

public enum AccessState
{
    Available,
    OverrideAvailable,
    Unavailable
}