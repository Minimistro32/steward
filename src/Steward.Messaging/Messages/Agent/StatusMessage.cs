namespace Steward.Messaging.Messages.Agent;

public sealed class StatusMessage
{
    /* steward/agents/test-agent/status
    {
        "status": "online"
    }*/

    public required AgentConnectionState State { get; init; }
}

public enum AgentConnectionState
{
    Offline,
    Online
}