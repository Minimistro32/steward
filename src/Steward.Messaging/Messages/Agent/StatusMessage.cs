namespace Steward.Messaging.Messages.Agent;

public sealed class StatusMessage
{
    /* steward/agents/test-agent/status
    {
        "state": "online"
    }*/

    public required AgentConnectionState State { get; init; }
}

public enum AgentConnectionState
{
    Offline,
    Online
}