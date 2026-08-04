namespace Steward.Server.Data.Entities;

public class UserDeviceEntity
{
    public int UserId { get; set; }

    public UserEntity User { get; set; } = null!;


    public int DeviceId { get; set; }

    public DeviceEntity Device { get; set; } = null!;
}