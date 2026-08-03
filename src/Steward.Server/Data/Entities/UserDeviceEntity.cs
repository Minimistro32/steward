namespace Steward.Server.Data.Entities;

public class UserDeviceEntity
{
    public string UserId { get; set; } = "";

    public UserEntity User { get; set; } = null!;


    public string DeviceId { get; set; } = "";

    public DeviceEntity Device { get; set; } = null!;
}