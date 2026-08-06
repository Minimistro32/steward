namespace Steward.Server.Application;

public sealed class PolicyEvaluation
{
    public required bool ScheduleActive { get; init; }

    // ACTIONABLE
    public required bool RequiresOverride { get; init; }
    public required int? MaxRequestMinutes { get; init; }

    // EXPLANATORY
    public required DateTimeOffset? ScheduleEndsAt { get; init; }
    public required int? EffectiveMinutesRemaining { get; init; }
    public required int? DailyMinutesRemaining { get; init; }
    public required int? UnlocksRemaining { get; init; }

}