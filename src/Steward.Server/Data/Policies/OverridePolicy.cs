namespace Steward.Server.Data.Policies;

public class OverridePolicy
{
    public bool Allowed { get; set; }

    public OverrideRequirement? Requirement { get; set; }

    public double DelayMinutes { get; set; } = 0.25;

    public int RandomTextLength { get; set; } = 30;

    public Allowance Allowance { get; set; } = new();
}

public enum OverrideRequirement
{
    Delay,
    RandomText,
    UserApproval
}