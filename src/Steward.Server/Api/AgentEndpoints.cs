using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Models;
using Steward.Server.Mqtt;

namespace Steward.Server.Api;

public static class AgentEndpoints
{
    public static void MapAgentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/agents");


        group.MapGet("/", async (StewardDbContext db) =>
        {
            var agents = await db.Agents
                .AsSplitQuery()
                .Include(a => a.Devices)
                .Include(a => a.Resources)
                .ToListAsync();

            return Results.Ok(
                agents.Select(AgentDto.FromEntity)
            );
        });

        group.MapPut("/{id}/toggle", async (
            string id,
            StewardDbContext db) =>
        {
            var agent = await db.Agents
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agent is null)
            {
                return Results.NotFound();
            }

            agent.Status?.State = agent.Status.State switch
            {
                AgentStatus.Disabled => AgentStatus.Offline,
                _ => AgentStatus.Disabled
            };

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        group.MapPost("/refresh", async (
            IMqttConnectionService mqtt) =>
        {
            await mqtt.PublishRefreshRequestAsync();

            return Results.Accepted();
        });
    }
}