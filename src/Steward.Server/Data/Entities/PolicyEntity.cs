using Steward.Server.Data.Policies;

namespace Steward.Server.Data.Entities;

public class PolicyEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public string Name { get; set; } = "";

    public ICollection<string> Tags { get; set; } = [];

    public bool Disabled { get; set; }

    public int WardId { get; set; }

    public WardEntity Ward { get; set; } = null!;

    public Schedule Schedule { get; set; } = new();

    public Allowance Access { get; set; } = new();

    public OverridePolicy Override { get; set; } = new();
}