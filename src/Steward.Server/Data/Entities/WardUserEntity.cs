namespace Steward.Server.Data.Entities;

public class WardUserEntity
{
    public int WardId { get; set; }

    public WardEntity Ward { get; set; } = null!;


    public int UserId { get; set; }

    public UserEntity User { get; set; } = null!;
}