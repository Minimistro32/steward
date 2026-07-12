namespace Steward.Messaging.Messages.Steward;

public sealed class CommandMessage
{
    public required string RequestId { get; init; }

    public required string ResourceId { get; init; }

    public required string Action { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}