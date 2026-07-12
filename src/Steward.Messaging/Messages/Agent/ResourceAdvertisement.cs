namespace Steward.Messaging.Messages.Agent;

public sealed class ResourceAdvertisement
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public List<string> Actions { get; init; } = [];
}