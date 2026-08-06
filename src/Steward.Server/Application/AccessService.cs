using Microsoft.EntityFrameworkCore;
using Steward.Server.Api.Models;
using Steward.Server.Data;

namespace Steward.Server.Application;

public sealed class AccessService(
    StewardDbContext db,
    PolicyEvaluator evaluator)
{
    public async Task<AccessDto?> GetAccessAsync(int userId)
    {
        /*
        Verify user exists

        Load candidate policies

        Load usage state for those policies

        foreach policy

            Evaluate policy

            Skip if not visible

            Build AccessOptionDto

        Return AccessDto
        */
        var userExists = await db.Users
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
            return null;


        //
        // Find policies that currently apply to this user.
        //
        // A policy is relevant if:
        // - it is enabled
        // - the user's membership connects them to the policy's ward
        // - the policy schedule is active
        //
        // TODO:
        // Move schedule filtering into a queryable policy evaluator
        // once schedule rules become more complex.
        //
        var policies = await db.Policies
            .AsSplitQuery()
            .Include(p => p.Ward)
                .ThenInclude(w => w.Resources)
                    .ThenInclude(wr => wr.Resource)
            .Include(p => p.Ward)
                .ThenInclude(w => w.Devices)
                    .ThenInclude(wd => wd.Device)
            .Where(p =>
                !p.Disabled &&
                p.Ward.Users.Any(wu =>
                    wu.UserId == userId))
            .ToListAsync();

        var policyIds = policies
            .Select(p => p.Id)
            .ToList();

        var policyAccess = await db.PolicyAccess
            .Where(pa =>
                pa.UserId == userId &&
                policyIds.Contains(pa.PolicyId))
            .ToDictionaryAsync(pa => pa.PolicyId);


        var options = new List<AccessOptionDto>();


        foreach (var policy in policies)
        {
            // get the user's usage for this policy
            policyAccess.TryGetValue(policy.Id, out var access);


            var evaluation = evaluator.Evaluate(
                policy,
                access);


            //
            // Policies that are not currently active
            // should not appear as available access options.
            //
            if (!evaluation.IsScheduled)
                continue;


            options.Add(
                new AccessOptionDto
                {
                    GrantedResources =
                        [.. policy.Ward.Resources
                            .Select(wr =>
                                new ResourceDto
                                {
                                    Id = wr.Resource.Id,
                                    ResourceId = wr.Resource.ResourceId,
                                    Name = wr.Resource.Name
                                })],

                    Devices =
                        [.. policy.Ward.Devices
                            .Select(wd =>
                                new DeviceDto
                                {
                                    Id = wd.Device.Id,
                                    DeviceId = wd.Device.DeviceId,
                                    Name = wd.Device.Name
                                })],

                    State = evaluation.State,

                    MaxRequestMinutes = evaluation.MaxRequestMinutes,

                    ScheduleEndsAt = evaluation.ScheduleEndsAt,

                    EffectiveMinutesRemaining = evaluation.EffectiveMinutesRemaining,
                    
                    DailyMinutesRemaining = evaluation.DailyMinutesRemaining,

                    UnlocksRemaining = evaluation.UnlocksRemaining
                });
        }


        return new AccessDto
        {
            Options = options
        };
    }
}