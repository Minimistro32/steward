namespace Steward.Server.Data.Entities;

public class ResourceEntity
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string AgentId { get; set; } = "";

    public AgentEntity Agent { get; set; } = null!;
}