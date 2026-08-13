namespace Steward.Server.Api.Models;

using Steward.Server.Data.Policies;

public enum AccessRequestStatus
{
    Granted,
    OverrideRequired,
    Pending,
    Unavailable
}

public sealed class AccessResponseDto
{
    public required AccessRequestStatus State { get; init; }

    public int? OverrideRequestId { get; init; }

    public OverrideRequirement? Requirement { get; init; }

    public DateTime? AvailableAt { get; init; }

    public string? ChallengeText { get; init; }
}