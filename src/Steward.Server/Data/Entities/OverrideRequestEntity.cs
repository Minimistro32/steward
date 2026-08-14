namespace Steward.Server.Data.Entities;

using Steward.Server.Data.Policies;


public class OverrideRequestEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PolicyId { get; set; }

    public int RequestedMinutes { get; set; }

    public OverrideRequirement? Requirement { get; set; }

    public OverrideRequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    // Used by Delay requirements.
    // Null for other requirement types.
    public DateTimeOffset? AvailableAt { get; set; }

    // Used by RandomText requirements.
    // Null for other requirement types.
    public string? ChallengeText { get; set; }

    // Used by Approval requirements.
    // Null for other requirement types.
    public int? ApprovedByUserId { get; set; }

    public UserEntity User { get; set; } = null!;

    public PolicyEntity Policy { get; set; } = null!;
    
    public UserEntity? ApprovedByUser { get; set; }
}

public enum OverrideRequestStatus
{
    Pending = 0,
    Granted = 1,
    Rejected = 2
}