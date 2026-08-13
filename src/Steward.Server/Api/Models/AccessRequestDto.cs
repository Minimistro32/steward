namespace Steward.Server.Api.Models;

public sealed class AccessRequestDto
{
    public required int PolicyId { get; init; }

    public required int RequestedMinutes { get; init; }
}