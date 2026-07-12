namespace Steward.Messaging.Messages.Agent;

public sealed class StatusMessage
{
    public required AgentStatus State { get; init; }
}

public enum AgentStatus
{
    Offline,
    Online
}