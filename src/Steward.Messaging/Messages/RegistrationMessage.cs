namespace Steward.Messaging.Messages;

public sealed class RegistrationMessage
{
    /* steward/agents/register
    {
        "version": "0.1.0",
        "agentId": "test-agent",
        "instanceId": "abc123",
        "name": "Test Agent",
        "devices": [
            {
                "deviceId": "desktop",
                "name": "Gaming PC"
            },
            {
                "deviceId": "laptop",
                "name": "Work Laptop"
            }
        ],
        "resources": [
            {
                "resourceId": "youtube",
                "name": "YouTube"
            },
            {
                "resourceId": "reddit",
                "name": "Reddit"
            }
        ]
    }*/

    public required string Version { get; init; }

    public required string AgentId { get; init; }

    public required string InstanceId { get; init; }

    public required string Name { get; init; }

    public List<DeviceAdvertisement> Devices { get; init; } = [];

    public List<ResourceAdvertisement> Resources { get; init; } = [];
}

public sealed class DeviceAdvertisement
{
    public required string DeviceId { get; init; }

    public required string Name { get; init; }
}

public sealed class ResourceAdvertisement
{
    public required string ResourceId { get; init; }

    public required string Name { get; init; }
}