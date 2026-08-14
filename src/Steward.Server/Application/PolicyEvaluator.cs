using Steward.Server.Data.Entities;
using Steward.Server.Data.Policies;

namespace Steward.Server.Application;

public sealed class PolicyEvaluator
{
    public PolicyEvaluation Evaluate(PolicyEntity policy, PolicyAccessEntity? access)
    {
        var now = DateTimeOffset.Now;

        var scheduleEndsAt = GetScheduleEnd(
            policy.Schedule,
            now);

        //
        // Policy is not currently applicable.
        //
        if (!IsScheduleActive(policy.Schedule, now))
        {
            return new PolicyEvaluation
            {
                IsScheduled = false,
                State = AccessState.Unavailable,
                MaxRequestMinutes = 0,
                ScheduleEndsAt = scheduleEndsAt,
                EffectiveMinutesRemaining = 0,
                DailyMinutesRemaining = 0,
                UnlocksRemaining = 0
            };
        }

        //
        // Usage resets automatically each day.
        //
        var minutesUsed = GetUsageFor(access?.LastAccessed, access?.MinutesUsed);
        var unlocksUsed = GetUsageFor(access?.LastAccessed, access?.UnlocksUsed);
        var overrideMinutesUsed = GetUsageFor(access?.LastAccessed, access?.OverrideMinutesUsed);
        var overrideUnlocksUsed = GetUsageFor(access?.LastAccessed, access?.OverrideUnlocksUsed);

        //
        // Daily allowances remaining.
        //
        int? dailyMinutesRemaining = GetRemainingFor(policy.Access.DailyTimeMinutes, minutesUsed);
        int? unlocksRemaining = GetRemainingFor(policy.Access.DailyUnlocks, unlocksUsed);
        int? overrideMinutesRemaining = GetRemainingFor(policy.Override.Allowance.DailyTimeMinutes, overrideMinutesUsed);
        int? overrideUnlocksRemaining = GetRemainingFor(policy.Override.Allowance.DailyUnlocks, overrideUnlocksUsed);

        //
        // Maximum request duration.
        //
        var maxSessionMinutes = policy.Access.MaxSessionMinutes;
        var overrideMaxSessionMinutes = policy.Override.Allowance.MaxSessionMinutes;

        //
        // Schedule remaining time.
        //
        var scheduleRemainingMinutes =
            GetRemainingScheduleMinutes(
                scheduleEndsAt,
                now);

        //
        // How much time can actually be consumed today?
        //
        // This accounts for:
        // - daily minute allowance
        // - number of remaining unlocks
        // - maximum session length
        // - schedule end
        //
        var effectiveMinutesRemaining =
            MinNullable(
                dailyMinutesRemaining,
                CalculateUnlockCapacity(
                    unlocksRemaining,
                    maxSessionMinutes),
                scheduleRemainingMinutes);

        var effectiveOverrideMinutesRemaining =
            MinNullable(
                overrideMinutesRemaining,
                CalculateUnlockCapacity(
                    overrideUnlocksRemaining,
                    overrideMaxSessionMinutes),
                scheduleRemainingMinutes);

        //
        // Maximum request right now.
        //
        var maxRequestMinutes = MinNullable(effectiveMinutesRemaining, maxSessionMinutes);
        var overrideMaxRequestMinutes = MinNullable(effectiveOverrideMinutesRemaining, overrideMaxSessionMinutes);


        //
        // Access is unavailable if allowance is exhausted and not override eligible.
        //
        AccessState state;
        if (effectiveMinutesRemaining == 0)
        {
            state = policy.Override.Allowed && effectiveOverrideMinutesRemaining != 0
                ? AccessState.OverrideAvailable
                : AccessState.Unavailable;
        }
        else
        {
            state = AccessState.Available;
        }

        return new PolicyEvaluation
        {
            IsScheduled = true,

            State = state,

            ScheduleEndsAt = scheduleEndsAt,

            MaxRequestMinutes =
                state switch
                {
                    AccessState.Available => maxRequestMinutes,
                    AccessState.OverrideAvailable => overrideMaxRequestMinutes,
                    _ => 0
                },

            DailyMinutesRemaining =
                state == AccessState.OverrideAvailable
                    ? overrideMinutesRemaining
                    : dailyMinutesRemaining,

            EffectiveMinutesRemaining =
                state == AccessState.OverrideAvailable
                    ? effectiveOverrideMinutesRemaining
                    : effectiveMinutesRemaining,

            UnlocksRemaining =
                state == AccessState.OverrideAvailable
                    ? overrideUnlocksRemaining
                    : unlocksRemaining
        };
    }

    private static int? CalculateUnlockCapacity(int? unlocksRemaining, int? maxSessionMinutes)
    {
        if (unlocksRemaining == 0)
            return 0;

        if (unlocksRemaining is null || maxSessionMinutes is null)
            return null;

        return unlocksRemaining.Value *
               maxSessionMinutes.Value;
    }


    private static int? MinNullable(params int?[] values)
    {
        var constrained =
            values
                .Where(v => v.HasValue)
                .Cast<int>()
                .ToList();

        return constrained.Count == 0
            ? null
            : constrained.Min();
    }


    private static bool IsScheduleActive(Schedule schedule, DateTimeOffset now)
    {
        if (!schedule.Days.Includes(now.DayOfWeek))
            return false;

        var currentTime =
            TimeOnly.FromDateTime(now.DateTime);

        // TODO: Support schedules that cross midnight.
        // Current assumption:
        // StartTime <= EndTime and both occur on the same day.

        return currentTime >= schedule.StartTime &&
               currentTime <= schedule.EndTime;
    }


    private static DateTimeOffset? GetScheduleEnd(Schedule schedule, DateTimeOffset now)
    {
        if (schedule.EndTime == TimeOnly.MaxValue)
            return null;

        return new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            schedule.EndTime.Hour,
            schedule.EndTime.Minute,
            schedule.EndTime.Second,
            now.Offset);
    }


    private static int? GetRemainingScheduleMinutes(DateTimeOffset? scheduleEndsAt, DateTimeOffset now)
    {
        if (scheduleEndsAt is null)
            return null;

        return Math.Max(
            0,
            (int)Math.Ceiling(
                (scheduleEndsAt.Value - now)
                    .TotalMinutes));
    }

    private static int GetUsageFor(DateOnly? lastAccessed, int? usage)
    {
        var now = DateTimeOffset.Now;
        var today = DateOnly.FromDateTime(now.Date);
        return lastAccessed == today ? (usage ?? 0) : 0;
    }

    private static int? GetRemainingFor(int? limit, int usage)
    {
        return limit is int limitInt ? Math.Max(0, limitInt - usage) : null;
    }
}