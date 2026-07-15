namespace Steward.Messaging.Messages.Agent;

public sealed class StatusMessage
{
    /* steward/agents/test-agent/status
    {
        "status": "online"
    }*/

    public required AgentStatus State { get; init; }
}

public enum AgentStatus
{
    Offline,
    Online
}