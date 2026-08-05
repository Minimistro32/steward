namespace Steward.Server.Data.Entities;

public class AgentEntity
{    
    public string Id { get; set; } = "";
    
    public string InstanceId { get; set; } = "";

    public string Version { get; set; } = "";

    public string Name { get; set; } = "";

    public AgentStatusEntity? Status { get; set; }

    public ICollection<DeviceEntity> Devices { get; set; } = [];

    public ICollection<ResourceEntity> Resources { get; set; } = [];
}