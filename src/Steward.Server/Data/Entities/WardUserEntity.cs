namespace Steward.Server.Data.Entities;

public class WardUserEntity
{
    public string WardId { get; set; } = "";

    public WardEntity Ward { get; set; } = null!;


    public string UserId { get; set; } = "";

    public UserEntity User { get; set; } = null!;
}