using Steward.Server.Authentication;
using Steward.Server.Data;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Api.Models;
using Steward.Server.Application;

namespace Steward.Server.Api;

public static class AccessEndpoints
{
    public static void MapAccessEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/access").RequireAuthorization();


        group.MapGet("/{userId}", async (
            int userId,
            AccessService access, HttpContext context) =>
        {
            if (!context.User.IsInRole("Admin") && context.User.UserId() != userId) return Results.Forbid();
            var response = await access.GetAccessAsync(userId);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost("/{userId}/request", async (
            int userId,
            AccessRequestDto dto,
            AccessService access, HttpContext context) =>
        {
            if (!context.User.IsInRole("Admin") && context.User.UserId() != userId) return Results.Forbid();
            var result = await access.RequestAccessAsync(userId, dto);
            return ToHttpResult(result);
        });


        group.MapPost("/{userId}/override", async (
            int userId,
            AccessRequestDto dto,
            AccessService access, HttpContext context) =>
        {
            if (!context.User.IsInRole("Admin") && context.User.UserId() != userId) return Results.Forbid();
            var result = await access.RequestOverrideAsync(userId, dto);
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/complete",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access, HttpContext context) =>
        {
            var db = context.RequestServices.GetRequiredService<StewardDbContext>();
            var owner = await db.OverrideRequests.Where(r => r.Id == requestId).Select(r => (int?)r.UserId).FirstOrDefaultAsync();
            if (owner is null) return Results.NotFound();
            if (!context.User.IsInRole("Admin") && owner != context.User.UserId()) return Results.Forbid();
            var result = await access.CompleteOverrideAsync(requestId, new OverrideActionDto { UserId = context.User.UserId(), ChallengeText = dto.ChallengeText });
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/approve",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access, HttpContext context) =>
        {
            if (!context.User.IsInRole("Admin")) return Results.Forbid();
            var result = await access.ApproveOverrideAsync(requestId, context.User.UserId());
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/reject",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access, HttpContext context) =>
        {
            if (!context.User.IsInRole("Admin")) return Results.Forbid();
            var db = context.RequestServices.GetRequiredService<StewardDbContext>();
            if (await db.OverrideRequests.AnyAsync(r => r.Id == requestId && r.UserId == context.User.UserId())) return Results.Forbid();
            var result = await access.RejectOverrideAsync(requestId);
            return ToHttpResult(result);
        });

        group.MapGet(
            "/requests",
            async (AccessService access, HttpContext context) =>
        {
            var requests =
                await access.GetRequestActivityAsync();

            return Results.Ok(requests);
        });
    }

    private static IResult ToHttpResult(AccessOperationResult result)
    {
        return result.Status switch
        {
            AccessOperationStatus.Success =>
                Results.Ok(result.Response),

            AccessOperationStatus.NotFound =>
                Results.NotFound(),

            AccessOperationStatus.Unauthorized =>
                Results.Forbid(),

            AccessOperationStatus.Forbidden =>
                Results.Forbid(),

            AccessOperationStatus.Invalid =>
                Results.BadRequest(),

            AccessOperationStatus.Conflict =>
                Results.Conflict(),

            _ =>
                Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}