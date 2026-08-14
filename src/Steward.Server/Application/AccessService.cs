using Microsoft.EntityFrameworkCore;
using Steward.Server.Api.Models;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Data.Policies;

namespace Steward.Server.Application;

public sealed class AccessService(
    StewardDbContext db,
    PolicyEvaluator evaluator)
{
    public async Task<AccessOptionsDto?> GetAccessAsync(int userId)
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
                    PolicyId = policy.Id,

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


        return new AccessOptionsDto
        {
            Options = options
        };
    }

    public async Task<AccessOperationResult> RequestAccessAsync(int userId, AccessRequestDto dto)
    {
        // A request for zero or negative minutes is invalid.
        if (dto.RequestedMinutes <= 0)
            return AccessOperationResult.Invalid();


        // load policy and access
        var context = await LoadPolicyContextAsync(
            userId,
            dto.PolicyId);

        if (context is null)
            return AccessOperationResult.NotFound();

        var policy = context.Value.Policy;
        var access = context.Value.Access;


        // IMPORTANT:
        // Re-evaluate at request time.
        var evaluation = evaluator.Evaluate(policy, access);


        //
        // Normal access is only available when the policy
        // evaluation says it is available.
        //
        if (evaluation.State == AccessState.OverrideAvailable)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.OverrideRequired
                });
        }

        if (evaluation.State == AccessState.Unavailable)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.Unavailable
                });
        }


        //
        // The evaluator says normal access is available.
        // Make sure the requested duration is within the
        // currently available allowance.
        //
        if (evaluation.MaxRequestMinutes is int maxRequestMinutes &&
            dto.RequestedMinutes > maxRequestMinutes)
        {
            return AccessOperationResult.Invalid();
        }


        //
        // Update or create today's usage record.
        //
        GrantAccess(
            access,
            userId,
            policy.Id,
            dto.RequestedMinutes,
            AccessGrantType.Normal);


        await db.SaveChangesAsync();


        return AccessOperationResult.Success(
            new AccessResponseDto
            {
                State = AccessRequestStatus.Granted
            });
    }

    public async Task<AccessOperationResult> RequestOverrideAsync(int userId, AccessRequestDto dto)
    {
        // A request for zero or negative minutes is invalid.
        if (dto.RequestedMinutes <= 0)
            return AccessOperationResult.Invalid();

        // Load the policy and the user's usage state.
        var context = await LoadPolicyContextAsync(userId, dto.PolicyId);

        if (context is null)
            return AccessOperationResult.NotFound();

        var policy = context.Value.Policy;
        var access = context.Value.Access;


        //
        // Re-evaluate the policy at request time.
        //
        var evaluation = evaluator.Evaluate(policy, access);

        if (evaluation.State == AccessState.Available)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.Invalid
                });
        }

        if (evaluation.State == AccessState.Unavailable)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.Unavailable
                });
        }


        //
        // Make sure the requested amount fits within the
        // currently available override allowance.
        //
        if (evaluation.MaxRequestMinutes is int maxRequestMinutes &&
            dto.RequestedMinutes > maxRequestMinutes)
        {
            return AccessOperationResult.Invalid();
        }


        //
        // There should only be one active override request
        // for a user/policy at a time.
        //
        var existingRequest = await db.OverrideRequests
            .FirstOrDefaultAsync(r =>
                r.UserId == userId &&
                r.PolicyId == policy.Id &&
                r.Status == OverrideRequestStatus.Pending);

        if (existingRequest is not null)
        {
            //
            // A repeat request updates the requested duration.
            //
            existingRequest.RequestedMinutes = dto.RequestedMinutes;

            switch (existingRequest.Requirement)
            {
                case OverrideRequirement.Delay:
                    //
                    // Restart the delay.
                    //
                    existingRequest.AvailableAt =
                        DateTimeOffset.UtcNow.AddMinutes(
                            // TODO: Replace with the configured
                            // override delay.
                            0.25);
                    break;

                case OverrideRequirement.RandomText:
                    //
                    // Generate a new challenge.
                    //
                    existingRequest.ChallengeText =
                        GenerateChallengeText();
                    break;

                default:
                    break;
            }

            await db.SaveChangesAsync();

            return AccessOperationResult.Success(
                ToPendingResponse(existingRequest));
        }

        var request = new OverrideRequestEntity
        {
            UserId = userId,
            PolicyId = policy.Id,
            RequestedMinutes = dto.RequestedMinutes,
            Requirement = policy.Override.Requirement,
            Status = OverrideRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };


        //
        // Configure requirement-specific information.
        //
        if (request.Requirement is null)
        {
            GrantAccess(
                access,
                userId,
                policy.Id,
                dto.RequestedMinutes,
                AccessGrantType.Override);

            request.Status = OverrideRequestStatus.Granted;
        }
        else
        {
            switch (request.Requirement)
            {
                case OverrideRequirement.Delay:
                    request.AvailableAt =
                        DateTimeOffset.UtcNow.AddMinutes(
                            // TODO: Replace with the configured
                            // override delay.
                            0.25);
                    break;

                case OverrideRequirement.RandomText:
                    request.ChallengeText =
                        GenerateChallengeText();
                    break;

                default:
                    break;
            }
        }


        db.OverrideRequests.Add(request);

        await db.SaveChangesAsync();


        return AccessOperationResult.Success(
            new AccessResponseDto
            {
                State = request.Status == OverrideRequestStatus.Granted
                    ? AccessRequestStatus.Granted
                    : AccessRequestStatus.Pending,
                OverrideRequestId = request.Id,
                Requirement = request.Requirement,
                AvailableAt = request.AvailableAt,
                ChallengeText = request.ChallengeText
            });
    }

    public async Task<AccessOperationResult> CompleteOverrideAsync(int requestId, OverrideActionDto dto)
    {
        var request = await db.OverrideRequests
            .Include(r => r.Policy)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.Id == requestId);

        if (request is null)
            return AccessOperationResult.NotFound();


        //
        // A request can only be completed once.
        //
        if (request.Status != OverrideRequestStatus.Pending)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = request.Status == OverrideRequestStatus.Granted
                        ? AccessRequestStatus.Granted
                        : AccessRequestStatus.Unavailable,

                    OverrideRequestId = request.Id,
                    Requirement = request.Requirement,
                    AvailableAt = request.AvailableAt,
                    ChallengeText = request.ChallengeText
                });
        }


        //
        // Load the user's current usage state.
        //
        var access = await db.PolicyAccess
            .FirstOrDefaultAsync(pa =>
                pa.UserId == request.UserId &&
                pa.PolicyId == request.PolicyId);


        //
        // Re-evaluate the policy at completion time.
        //
        if (!CanGrantOverride(request, access))
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.Unavailable,
                    OverrideRequestId = request.Id
                });
        }


        //
        // Verify that the requirement has actually been satisfied.
        //
        switch (request.Requirement)
        {
            case OverrideRequirement.Delay:

                if (request.AvailableAt is null ||
                    DateTimeOffset.UtcNow < request.AvailableAt.Value)
                {
                    return AccessOperationResult.Success(
                        new AccessResponseDto
                        {
                            State = AccessRequestStatus.Pending,
                            OverrideRequestId = request.Id,
                            Requirement = request.Requirement,
                            AvailableAt = request.AvailableAt
                        });
                }

                break;


            case OverrideRequirement.RandomText:

                if (string.IsNullOrWhiteSpace(dto.ChallengeText) ||
                    dto.ChallengeText != request.ChallengeText)
                {
                    return AccessOperationResult.Invalid();
                }

                break;


            case OverrideRequirement.UserApproval:

                //
                // Approval requests must use the approve endpoint.
                //
                return AccessOperationResult.Forbidden();
        }


        //
        // The requirement has been satisfied.
        // Grant the requested override and record it.
        //
        GrantAccess(
            access,
            request.UserId,
            request.PolicyId,
            request.RequestedMinutes,
            AccessGrantType.Override);

        request.Status = OverrideRequestStatus.Granted;


        await db.SaveChangesAsync();


        return AccessOperationResult.Success(
            new AccessResponseDto
            {
                State = AccessRequestStatus.Granted,
                OverrideRequestId = request.Id
            });
    }

    public async Task<AccessOperationResult> ApproveOverrideAsync(int requestId, int userId)
    {
        var request = await db.OverrideRequests
            .Include(r => r.Policy)
            .FirstOrDefaultAsync(r =>
                r.Id == requestId);

        if (request is null)
            return AccessOperationResult.NotFound();


        var userExists = await db.Users
            .AnyAsync(u => u.Id == userId);

        if (!userExists || request.UserId == userId)
            return AccessOperationResult.Unauthorized();


        // Only pending approval requests can be approved.
        if (request.Status != OverrideRequestStatus.Pending)
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = request.Status == OverrideRequestStatus.Granted
                        ? AccessRequestStatus.Granted
                        : AccessRequestStatus.Unavailable,

                    OverrideRequestId = request.Id
                });


        // Only UserApproval requests can be approved here.
        if (request.Requirement != OverrideRequirement.UserApproval)
            return AccessOperationResult.Forbidden();


        // Re-load current usage.
        var access = await db.PolicyAccess
            .FirstOrDefaultAsync(pa =>
                pa.UserId == request.UserId &&
                pa.PolicyId == request.PolicyId);


        // Re-evaluate before granting.
        if (!CanGrantOverride(request, access))
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = AccessRequestStatus.Unavailable,
                    OverrideRequestId = request.Id
                });
        }


        GrantAccess(
            access,
            request.UserId,
            request.PolicyId,
            request.RequestedMinutes,
            AccessGrantType.Override);

        request.Status = OverrideRequestStatus.Granted;
        request.ApprovedByUserId = userId;

        await db.SaveChangesAsync();


        return AccessOperationResult.Success(
            new AccessResponseDto
            {
                State = AccessRequestStatus.Granted,
                OverrideRequestId = request.Id
            });
    }

    public async Task<AccessOperationResult> RejectOverrideAsync(int requestId)
    {
        var request = await db.OverrideRequests
            .FirstOrDefaultAsync(r =>
                r.Id == requestId);

        if (request is null)
            return AccessOperationResult.NotFound();


        // Only pending requests can be rejected.
        if (request.Status != OverrideRequestStatus.Pending)
        {
            return AccessOperationResult.Success(
                new AccessResponseDto
                {
                    State = request.Status == OverrideRequestStatus.Granted
                        ? AccessRequestStatus.Granted
                        : AccessRequestStatus.Unavailable,

                    OverrideRequestId = request.Id
                });
        }


        // Only approval requests need an approver/rejecter.
        if (request.Requirement != OverrideRequirement.UserApproval)
            return AccessOperationResult.Forbidden();


        request.Status = OverrideRequestStatus.Rejected;

        await db.SaveChangesAsync();


        return AccessOperationResult.Success(
            new AccessResponseDto
            {
                State = AccessRequestStatus.Unavailable,
                OverrideRequestId = request.Id
            });
    }

    // HELPERS
    private async Task<(PolicyEntity Policy, PolicyAccessEntity? Access)?> LoadPolicyContextAsync(int userId, int policyId)
    {
        // policy applies to user
        var policy = await db.Policies
            .FirstOrDefaultAsync(p =>
                p.Id == policyId &&
                !p.Disabled &&
                p.Ward.Users.Any(wu =>
                    wu.UserId == userId));

        if (policy is null)
            return null;

        // Load the user's usage for this policy.
        var access = await db.PolicyAccess
            .FirstOrDefaultAsync(pa =>
                pa.UserId == userId &&
                pa.PolicyId == policyId);


        return (policy, access);
    }

    private static void ResetDailyUsageIfNeeded(PolicyAccessEntity access, DateOnly today)
    {
        if (access.LastAccessed == today)
            return;

        access.LastAccessed = today;

        access.MinutesUsed = 0;
        access.UnlocksUsed = 0;
        access.OverrideMinutesUsed = 0;
        access.OverrideUnlocksUsed = 0;
    }

    private static AccessResponseDto ToPendingResponse(OverrideRequestEntity request)
    {
        return new AccessResponseDto
        {
            State = AccessRequestStatus.Pending,
            OverrideRequestId = request.Id,
            Requirement = request.Requirement,
            AvailableAt = request.AvailableAt,
            ChallengeText = request.ChallengeText
        };
    }

    private static string GenerateChallengeText()
    {
        // TODO: Generate a random challenge text.
        return "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
    }

    private bool CanGrantOverride(OverrideRequestEntity request, PolicyAccessEntity? access)
    {
        var evaluation = evaluator.Evaluate(request.Policy, access);

        if (evaluation.State != AccessState.OverrideAvailable)
            return false;

        if (evaluation.MaxRequestMinutes is int maxRequestMinutes &&
            request.RequestedMinutes > maxRequestMinutes)
            return false;

        return true;
    }

    private void GrantAccess(PolicyAccessEntity? access, int userId, int policyId, int requestedMinutes, AccessGrantType grantType)
    {
        var today = DateOnly.FromDateTime(
            DateTimeOffset.Now.Date);

        if (access is null)
        {
            access = new PolicyAccessEntity
            {
                UserId = userId,
                PolicyId = policyId,
                LastAccessed = today
            };

            db.PolicyAccess.Add(access);
        }
        else
        {
            ResetDailyUsageIfNeeded(
                access,
                today);
        }

        if (grantType == AccessGrantType.Override)
        {
            access.OverrideMinutesUsed += requestedMinutes;
            access.OverrideUnlocksUsed += 1;
        }
        else
        {
            access.MinutesUsed += requestedMinutes;
            access.UnlocksUsed += 1;
        }
    }

    private enum AccessGrantType
    {
        Normal,
        Override
    }
}