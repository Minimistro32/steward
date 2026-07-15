namespace Steward.Messaging.Messages.Agent;

public sealed class RegistrationMessage
{
    /* steward/agents/register
    {
        "agentId": "test-agent",
        "instanceId": "abc123",
        "name": "Test Agent",
        "resources": [
            {
                "id": "1",
                "name": "media",
                "actions": ["block"]
            }
        ]
    }*/

    public required string AgentId { get; init; }

    public required string InstanceId { get; init; }

    public required string Name { get; init; }

    public List<ResourceAdvertisement> Resources { get; init; } = [];
}