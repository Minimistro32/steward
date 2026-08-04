namespace Steward.Server.Data.Entities;

public class DeviceEntity
{
    public int Id { get; set; }
    
    public string DeviceId { get; set; } = "";

    public string Name { get; set; } = "";

    public string AgentId { get; set; } = "";

    public AgentEntity Agent { get; set; } = null!;
}