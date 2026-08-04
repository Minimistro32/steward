namespace Steward.Server.Data.Entities;

public class WardResourceEntity
{
    public int WardId { get; set; }

    public WardEntity Ward { get; set; } = null!;


    public int ResourceId { get; set; }

    public ResourceEntity Resource { get; set; } = null!;
}