namespace Steward.Server.Data.Entities;

public class WardEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public ICollection<string> Tags { get; set; } = [];

    public ICollection<WardUserEntity> Users { get; set; } = [];

    public ICollection<WardDeviceEntity> Devices { get; set; } = [];

    public ICollection<WardResourceEntity> Resources { get; set; } = [];

    public ICollection<PolicyEntity> Policies { get; set; } = [];
}
