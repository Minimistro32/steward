namespace Steward.Server.Data.Entities;

// A durable marker: removing users must not reopen initial setup.
public class SetupStateEntity
{
    public int Id { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
}
