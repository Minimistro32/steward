namespace Steward.Server.Api.Models;

public sealed class AccessRequestDto
{
    public string? Reason { get; set; }

    public required int PolicyId { get; init; }

    public required int RequestedMinutes { get; init; }
}