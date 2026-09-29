using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Steward.Messaging;
using Steward.Messaging.Messages;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Mqtt.Handlers;

var directory = Path.Combine(Path.GetTempPath(), $"steward-mqtt-{Guid.NewGuid():N}");
Directory.CreateDirectory(directory);
try
{
    var options = new DbContextOptionsBuilder<StewardDbContext>()
        .UseSqlite($"Data Source={Path.Combine(directory, "test.db")};Pooling=False").Options;
    var factory = new TestDbFactory(options);
    await using (var db = factory.CreateDbContext()) await db.Database.MigrateAsync();
    var status = new StatusMessageHandler(NullLogger<StatusMessageHandler>.Instance, factory);
    var registration = new RegistrationMessageHandler(NullLogger<RegistrationMessageHandler>.Instance, factory);
    await status.HandleAsync("unknown", new StatusMessage { State = AgentConnectionState.Online });
    await status.HandleAsync("unknown", new StatusMessage { State = AgentConnectionState.Offline });
    await using (var db = factory.CreateDbContext())
        Check(!await db.Agents.AnyAsync() && !await db.AgentStatuses.AnyAsync(), "Unknown status must not create orphan rows or placeholder agents.");

    var message = new RegistrationMessage { AgentId = "unknown", InstanceId = "one", Name = "Test", Version = "1" };
    await registration.HandleAsync(StewardMessage.Serialize(message));
    await CheckState(AgentStatus.Online);
    await status.HandleAsync("unknown", new StatusMessage { State = AgentConnectionState.Offline });
    await CheckState(AgentStatus.Offline);
    await registration.HandleAsync(StewardMessage.Serialize(message));
    await CheckState(AgentStatus.Online);
    await using (var db = factory.CreateDbContext())
    {
        var current = await db.AgentStatuses.SingleAsync();
        current.State = AgentStatus.Disabled;
        await db.SaveChangesAsync();
    }
    await status.HandleAsync("unknown", new StatusMessage { State = AgentConnectionState.Online });
    await registration.HandleAsync(StewardMessage.Serialize(message));
    await CheckState(AgentStatus.Disabled);
    await registration.HandleAsync(StewardMessage.Serialize(new RegistrationMessage { AgentId = "unknown", InstanceId = "conflict", Name = "Wrong", Version = "2" }));
    await using (var db = factory.CreateDbContext())
    {
        Check((await db.Agents.SingleAsync()).Name == "Test", "Conflicting registration must not update the agent.");
        db.AgentStatuses.Remove(await db.AgentStatuses.SingleAsync());
        await db.SaveChangesAsync();
    }
    await status.HandleAsync("unknown", new StatusMessage { State = AgentConnectionState.Online });
    await CheckState(AgentStatus.Online);
    Console.WriteLine("PASS: status before registration, online/offline updates, refresh registration, disabled preservation, instance conflict, and missing-status recovery.");

    async Task CheckState(AgentStatus expected)
    {
        await using var db = factory.CreateDbContext();
        var entry = await db.AgentStatuses.SingleAsync();
        Check(entry.State == expected && entry.LastContact > DateTime.UtcNow.AddMinutes(-1), $"Expected {expected} with recent contact.");
    }
}
finally { Directory.Delete(directory, recursive: true); }

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class TestDbFactory(DbContextOptions<StewardDbContext> options) : IDbContextFactory<StewardDbContext>
{
    public StewardDbContext CreateDbContext() => new(options);
}
