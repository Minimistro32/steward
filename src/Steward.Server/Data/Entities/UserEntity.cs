namespace Steward.Server.Data.Entities;

public class UserEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; } = "";
    
    // Store only a salted credential hash, never the PIN itself.
    // Members may leave their PIN unset (null); admins must have a PIN hash.
    public string? PinHash { get; set; }

    public string? Email { get; set; }

    public UserType Type { get; set; } = UserType.Member;

    public ICollection<UserDeviceEntity> UserDevices { get; set; } = [];
}