namespace Steward.Server.Data.Entities;

public class UserEntity
{
    public string Id { get; set; } = "";
    
    public string Name { get; set; } = "";
    
    public List<UserDeviceEntity> UserDevices { get; set; } = [];
}