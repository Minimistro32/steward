namespace Steward.Server.Data.Entities;

public class WardDeviceEntity
{
    public int WardId { get; set; }

    public WardEntity Ward { get; set; } = null!;


    public int DeviceId { get; set; }

    public DeviceEntity Device { get; set; } = null!;
}