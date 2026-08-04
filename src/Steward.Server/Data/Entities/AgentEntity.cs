namespace Steward.Server.Data.Entities;

public class AgentEntity
{
    public int Id { get; set; }
    
    public string Version { get; set; } = "";
    
    public string AgentId { get; set; } = "";

    public string InstanceId { get; set; } = "";

    public string Name { get; set; } = "";

    public AgentStatus Status { get; set; }

    public DateTime? LastSeen { get; set; }


    public ICollection<DeviceEntity> Devices { get; set; } = [];

    public ICollection<ResourceEntity> Resources { get; set; } = [];
}

public enum AgentStatus
{
    Offline,
    Online,
    Disabled
}