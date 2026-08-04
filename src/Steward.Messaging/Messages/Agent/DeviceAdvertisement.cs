namespace Steward.Messaging.Messages.Agent;

public sealed class DeviceAdvertisement
{
    public required string DeviceId { get; init; }

    public required string Name { get; init; }
}