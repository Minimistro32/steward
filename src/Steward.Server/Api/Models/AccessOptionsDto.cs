using Steward.Server.Application;

namespace Steward.Server.Api.Models;

public sealed class AccessOptionsDto
{
    public required IReadOnlyCollection<AccessOptionDto> Options { get; init; }
}

public sealed class AccessOptionDto
{
    public required int PolicyId { get; init; }
    
    public required IReadOnlyCollection<ResourceDto> GrantedResources { get; init; }

    public required IReadOnlyCollection<DeviceDto> Devices { get; init; }

    public required AccessState State { get; init; }

    public required int? MaxRequestMinutes { get; init; }

    public required DateTimeOffset? ScheduleEndsAt { get; init; }

    public required int? EffectiveMinutesRemaining { get; init; }

    public required int? DailyMinutesRemaining { get; init; }

    public required int? UnlocksRemaining { get; init; }
}