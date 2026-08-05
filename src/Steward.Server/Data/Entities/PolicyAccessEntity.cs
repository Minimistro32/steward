namespace Steward.Server.Data.Entities;

public class PolicyAccessEntity
{
    public int PolicyId { get; set; }

    public int UserId { get; set; }

    public DateOnly LastAccessed { get; set; }

    public int MinutesUsed { get; set; }

    public int UnlocksUsed { get; set; }

    public PolicyEntity Policy { get; set; } = null!;

    public UserEntity User { get; set; } = null!;
}