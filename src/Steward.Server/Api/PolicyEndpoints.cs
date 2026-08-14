using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Api.Models;
using Steward.Server.Data.Policies;

namespace Steward.Server.Api;

public static class PolicyEndpoints
{
    public static void MapPolicyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/policies");

        //
        // Get all policies
        //
        group.MapGet("/", async (StewardDbContext db) =>
        {
            var policies = await db.Policies
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(
                policies.Select(PolicyDto.FromEntity)
            );
        });


        //
        // Get policy by id
        //
        group.MapGet("/{id}", async (
            string id,
            StewardDbContext db) =>
        {
            var policy = await db.Policies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id.ToString() == id);

            if (policy is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(
                PolicyDto.FromEntity(policy)
            );
        });


        //
        // Create policy
        //
        group.MapPost("/", async (
            PolicyDto dto,
            StewardDbContext db) =>
        {
            var now = DateTime.UtcNow;

            var policy = new PolicyEntity
            {
                CreatedAt = now,

                ModifiedAt = now,

                Name = dto.Name,

                Tags = [.. dto.Tags],

                Disabled = dto.Disabled,

                WardId = dto.WardId,

                Schedule = dto.Schedule.ToSchedule(),

                Access = dto.Access.ToAllowance(),

                Override = new OverridePolicy
                {
                    Allowed = dto.Override.Allowed,

                    Requirement = dto.Override.Requirement,

                    Allowance = dto.Override.Allowance.ToAllowance()
                }
            };

            db.Policies.Add(policy);

            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/policies/{policy.Id}",
                PolicyDto.FromEntity(policy)
            );
        });


        //
        // Update policy
        //
        group.MapPut("/{id}", async (
            string id,
            PolicyDto dto,
            StewardDbContext db) =>
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.Id.ToString() == id);

            if (policy is null)
            {
                return Results.NotFound();
            }

            var oldRequirement = policy.Override.Requirement;
            var newRequirement = dto.Override.Requirement;

            if (oldRequirement != newRequirement)
            {
                var pendingRequests = await db.OverrideRequests
                    .Where(r =>
                        r.PolicyId == policy.Id &&
                        r.Status == OverrideRequestStatus.Pending)
                    .ToListAsync();

                foreach (var request in pendingRequests)
                {
                    request.Status = OverrideRequestStatus.Rejected;
                }
            }

            policy.Name = dto.Name;

            policy.Tags = [.. dto.Tags];

            policy.Disabled = dto.Disabled;

            policy.WardId = dto.WardId;

            policy.Schedule = dto.Schedule.ToSchedule();

            policy.Access = dto.Access.ToAllowance();

            policy.Override = new OverridePolicy
            {
                Allowed = dto.Override.Allowed,
                Requirement = newRequirement,
                Allowance = dto.Override.Allowance.ToAllowance()
            };

            policy.ModifiedAt = DateTime.UtcNow;


            await db.SaveChangesAsync();

            return Results.NoContent();
        });


        //
        // Delete policy
        //
        group.MapDelete("/{id}", async (
            string id,
            StewardDbContext db) =>
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.Id.ToString() == id);

            if (policy is null)
            {
                return Results.NotFound();
            }


            db.Policies.Remove(policy);

            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}