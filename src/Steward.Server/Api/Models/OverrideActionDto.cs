namespace Steward.Server.Api.Models;

public sealed class OverrideActionDto
{
    public required int UserId { get; init; }

    public string? ChallengeText { get; init; }
}