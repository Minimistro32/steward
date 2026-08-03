namespace Steward.Server.Data.Entities;

public class WardDeviceEntity
{
    public string WardId { get; set; } = "";

    public WardEntity Ward { get; set; } = null!;


    public string DeviceId { get; set; } = "";

    public DeviceEntity Device { get; set; } = null!;
}