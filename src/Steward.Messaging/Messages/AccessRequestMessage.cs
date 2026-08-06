namespace Steward.Messaging.Messages;

public sealed class AccessRequestMessage
{
    public required string RequestId { get; init; }

    public required IReadOnlyCollection<string> DeviceIds { get; init; }

    public required IReadOnlyCollection<string> ResourceIds { get; init; }

    public required DateTimeOffset AllowedUntil { get; init; }
}