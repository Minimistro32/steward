using Microsoft.EntityFrameworkCore;
using Steward.Messaging;
using Steward.Messaging.Messages;
using Steward.Server.Data;
using Steward.Server.Data.Entities;

namespace Steward.Server.Mqtt.Handlers;

public sealed class StatusMessageHandler(
    ILogger<StatusMessageHandler> logger,
    IDbContextFactory<StewardDbContext> dbFactory)
{
    private readonly ILogger<StatusMessageHandler> logger = logger;
    private readonly IDbContextFactory<StewardDbContext> dbFactory = dbFactory;

    public async Task HandleAsync(
        string agentId,
        StatusMessage message)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var status = await db.AgentStatuses
            .FirstOrDefaultAsync(
                s => s.AgentId == agentId);

        AgentStatus getAgentStatus() => message.State == AgentConnectionState.Online ? AgentStatus.Online : AgentStatus.Offline;

        if (status is null)
        {
            status = new AgentStatusEntity
            {
                AgentId = agentId,
                State = getAgentStatus(),
                LastContact = DateTime.UtcNow
            };

            db.AgentStatuses.Add(status);
        }
        else
        {
            if (status.State != AgentStatus.Disabled) {
                status.State = getAgentStatus();
            }
            status.LastContact = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }
}