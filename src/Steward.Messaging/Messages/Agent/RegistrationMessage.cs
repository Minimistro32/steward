namespace Steward.Messaging.Messages.Agent;

public sealed class RegistrationMessage
{
    public required string AgentId { get; init; }

    public required string InstanceId { get; init; }

    public required string Name { get; init; }

    public List<ResourceAdvertisement> Resources { get; init; } = [];
}