namespace Steward.Server.Api.Models;

using Steward.Server.Data.Policies;
using Steward.Server.Data.Entities;

public sealed class RequestActivityDto
{
    public string? Reason { get; set; }

    public required int Id { get; init; }

    public required int UserId { get; init; }

    public required string UserName { get; init; }

    public required OverrideRequirement? Requirement { get; init; }

    public required OverrideRequestStatus Status { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required int RequestedMinutes { get; init; }

    public required List<string> Resources { get; init; }
}