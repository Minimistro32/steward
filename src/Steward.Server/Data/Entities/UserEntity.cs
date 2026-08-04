namespace Steward.Server.Data.Entities;

public class UserEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = "";
    
    public ICollection<UserDeviceEntity> UserDevices { get; set; } = [];
}