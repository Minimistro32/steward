namespace Steward.Server.Data.Entities;

public class WardResourceEntity
{
    public string WardId { get; set; } = "";

    public WardEntity Ward { get; set; } = null!;


    public string ResourceId { get; set; } = "";

    public ResourceEntity Resource { get; set; } = null!;
}