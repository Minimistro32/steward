namespace Steward.Server.Data.Policies;

public class OverridePolicy
{
    public bool Allowed { get; set; }

    public OverrideRequirement? Requirement { get; set; }

    public Allowance Allowance { get; set; } = new();
}

public enum OverrideRequirement
{
    Delay,
    RandomText,
    UserApproval
}