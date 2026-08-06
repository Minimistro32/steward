using Steward.Server.Data.Entities;
using Steward.Server.Data.Policies;

namespace Steward.Server.Api.Models;

public class PolicyDto
{
    public int? Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }


    public string Name { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public bool Disabled { get; set; }


    public int WardId { get; set; }


    public ScheduleDto Schedule { get; set; } = new();

    public AllowanceDto Access { get; set; } = new();

    public OverridePolicyDto Override { get; set; } = new();



    public static PolicyDto FromEntity(PolicyEntity policy) => new()
    {
        Id = policy.Id,

        CreatedAt = policy.CreatedAt,

        ModifiedAt = policy.ModifiedAt,

        Name = policy.Name,

        Tags = [.. policy.Tags],

        Disabled = policy.Disabled,

        WardId = policy.WardId,

        Schedule = new ScheduleDto
        {
            Days = [.. policy.Schedule.Days.ToDayList()],

            StartTime =
                policy.Schedule.StartTime == TimeOnly.MinValue
                    ? ""
                    : policy.Schedule.StartTime.ToString("HH:mm"),

            EndTime =
                policy.Schedule.EndTime == TimeOnly.MaxValue
                    ? ""
                    : policy.Schedule.EndTime.ToString("HH:mm")
        },

        Access = AllowanceDto.FromEntity(policy.Access),

        Override = new OverridePolicyDto
        {
            Allowed = policy.Override.Allowed,

            Requirement = policy.Override.Requirement,

            Allowance = AllowanceDto.FromEntity(policy.Override.Allowance)
        }
    };
}


public class ScheduleDto
{
    public List<DayOfWeek> Days { get; set; } = [];

    // Matches frontend expectations: ""
    // means start/end of day
    public string StartTime { get; set; } = "";

    public string EndTime { get; set; } = "";

    public Schedule ToSchedule()
    {
        return new Schedule
        {
            Days = Days.ToWeekDays(),

            StartTime = ParseStartTime(StartTime),

            EndTime = ParseEndTime(EndTime)
        };
    }

    private static TimeOnly ParseStartTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TimeOnly.MinValue;

        return TimeOnly.Parse(value);
    }


    private static TimeOnly ParseEndTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TimeOnly.MaxValue;

        return TimeOnly.Parse(value);
    }
}

public class AllowanceDto
{
    public int? DailyTimeMinutes { get; set; }

    public int? MaxSessionMinutes { get; set; }

    public int? DailyUnlocks { get; set; }


    public static AllowanceDto FromEntity(Allowance allowance) => new()
    {
        DailyTimeMinutes = allowance.DailyTimeMinutes,

        MaxSessionMinutes = allowance.MaxSessionMinutes,

        DailyUnlocks = allowance.DailyUnlocks
    };

    public Allowance ToAllowance()
    {
        return new Allowance
        {
            DailyTimeMinutes = DailyTimeMinutes,

            MaxSessionMinutes = MaxSessionMinutes,

            DailyUnlocks = DailyUnlocks
        };
    }
}


public class OverridePolicyDto
{
    public bool Allowed { get; set; }

    public OverrideRequirement? Requirement { get; set; }

    public AllowanceDto Allowance { get; set; } = new();
}