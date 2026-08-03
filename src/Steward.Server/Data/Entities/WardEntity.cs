namespace Steward.Server.Data.Entities;

public class WardEntity
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public List<WardUserEntity> Users { get; set; } = [];

    public List<WardDeviceEntity> Devices { get; set; } = [];

    public List<WardResourceEntity> Resources { get; set; } = [];
}
