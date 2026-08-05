namespace Steward.Server.Data.Entities;

public class AgentStatusEntity
{
    public string AgentId { get; set; } = "";

    public AgentEntity Agent { get; set; } = null!;

    public AgentStatus State { get; set; }

    public DateTime LastSeen { get; set; }
}

public enum AgentStatus
{
    Offline,
    Online,
    Disabled
}