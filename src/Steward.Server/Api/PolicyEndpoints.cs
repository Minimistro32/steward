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
        var group = app.MapGroup("/api/policies").RequireAuthorization("Admin");

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
            var errors = ValidateOverride(dto.Override);
            if (errors.Count > 0) return Results.ValidationProblem(errors);
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
                    DelayMinutes = dto.Override.DelayMinutes,
                    RandomTextLength = dto.Override.RandomTextLength,

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

            var errors = ValidateOverride(dto.Override);
            if (errors.Count > 0) return Results.ValidationProblem(errors);
            var oldRequirement = policy.Override.Requirement;
            var newRequirement = dto.Override.Requirement;

            if (oldRequirement != newRequirement || policy.Override.Allowed != dto.Override.Allowed
                || (newRequirement == OverrideRequirement.Delay && policy.Override.DelayMinutes != dto.Override.DelayMinutes)
                || (newRequirement == OverrideRequirement.RandomText && policy.Override.RandomTextLength != dto.Override.RandomTextLength))
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
                DelayMinutes = dto.Override.DelayMinutes,
                RandomTextLength = dto.Override.RandomTextLength,
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
    private static Dictionary<string, string[]> ValidateOverride(OverridePolicyDto value)
    {
        var errors = new Dictionary<string, string[]>();
        if (!double.IsFinite(value.DelayMinutes) || value.DelayMinutes < 0.01 || value.DelayMinutes > 1440)
            errors["delayMinutes"] = ["Delay must be between 0.01 and 1440 minutes."];
        if (value.RandomTextLength < 3 || value.RandomTextLength > 2000)
            errors["randomTextLength"] = ["Random text length must be between 3 and 2000 characters."];
        if (value.Requirement is not null && !Enum.IsDefined(value.Requirement.Value))
            errors["requirement"] = ["Choose a valid override requirement."];
        return errors;
    }
}