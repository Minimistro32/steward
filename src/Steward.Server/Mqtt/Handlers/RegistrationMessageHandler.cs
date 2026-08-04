using Microsoft.EntityFrameworkCore;
using Steward.Messaging;
using Steward.Messaging.Messages.Agent;
using Steward.Server.Data;
using Steward.Server.Data.Entities;

namespace Steward.Server.Mqtt.Handlers;

public sealed class RegistrationMessageHandler(
    ILogger<RegistrationMessageHandler> logger,
    IDbContextFactory<StewardDbContext> dbFactory)
{
    private readonly ILogger<RegistrationMessageHandler> logger = logger;
    private readonly IDbContextFactory<StewardDbContext> dbFactory = dbFactory;

    public async Task HandleAsync(string json)
    {
        RegistrationMessage? registration;

        try
        {
            registration =
                StewardMessage.Deserialize<RegistrationMessage>(json);

            if (registration is null)
            {
                logger.LogWarning(
                    "Failed to deserialize registration message.");

                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to deserialize registration message.");

            return;
        }

        logger.LogInformation(
            "Processing registration for {AgentId}",
            registration.AgentId);

        await using var db =
            await dbFactory.CreateDbContextAsync();

        var agent = await db.Agents
            .Include(a => a.Devices)
            .Include(a => a.Resources)
            .SingleOrDefaultAsync(a =>
                a.AgentId == registration.AgentId);

        if (agent is null)
        {
            agent = new AgentEntity
            {
                AgentId = registration.AgentId
            };

            db.Agents.Add(agent);
        }

        agent.InstanceId = registration.InstanceId;
        agent.Name = registration.Name;
        agent.Version = registration.Version;
        agent.Status = AgentStatus.Online;
        agent.LastSeen = DateTime.UtcNow;

        SynchronizeDevices(agent, registration);

        SynchronizeResources(agent, registration);

        await db.SaveChangesAsync();

        logger.LogInformation(
            "Saved agent {AgentId} to database.",
            agent.AgentId);
    }

    private static void SynchronizeDevices(
        AgentEntity agent,
        RegistrationMessage registration)
    {
        var advertised =
            registration.Devices.ToDictionary(d => d.DeviceId);

        foreach (var device in agent.Devices.ToList())
        {
            if (!advertised.ContainsKey(device.DeviceId))
            {
                // TODO: REEVAULATE THIS
                agent.Devices.Remove(device);
            }
        }

        foreach (var incoming in registration.Devices)
        {
            var existing =
                agent.Devices.SingleOrDefault(d =>
                    d.DeviceId == incoming.DeviceId);

            if (existing is null)
            {
                agent.Devices.Add(new DeviceEntity
                {
                    DeviceId = incoming.DeviceId,
                    Name = incoming.Name
                });
            }
            else
            {
                existing.Name = incoming.Name;
            }
        }
    }

    private static void SynchronizeResources(
        AgentEntity agent,
        RegistrationMessage registration)
    {
        var advertised =
            registration.Resources.ToDictionary(r => r.ResourceId);

        foreach (var resource in agent.Resources.ToList())
        {
            if (!advertised.ContainsKey(resource.ResourceId))
            {
                // TODO: REEVAULATE THIS
                agent.Resources.Remove(resource);
            }
        }

        foreach (var incoming in registration.Resources)
        {
            var existing =
                agent.Resources.SingleOrDefault(r =>
                    r.ResourceId == incoming.ResourceId);

            if (existing is null)
            {
                agent.Resources.Add(new ResourceEntity
                {
                    ResourceId = incoming.ResourceId,
                    Name = incoming.Name
                });
            }
            else
            {
                existing.Name = incoming.Name;
            }
        }
    }
}