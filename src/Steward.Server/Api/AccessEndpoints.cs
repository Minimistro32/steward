using Steward.Server.Api.Models;
using Steward.Server.Application;

namespace Steward.Server.Api;

public static class AccessEndpoints
{
    public static void MapAccessEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/access");


        group.MapGet("/{userId}", async (
            int userId,
            AccessService access) =>
        {
            var response = await access.GetAccessAsync(userId);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost("/{userId}/request", async (
            int userId,
            AccessRequestDto dto,
            AccessService access) =>
        {
            var result = await access.RequestAccessAsync(userId, dto);
            return ToHttpResult(result);
        });


        group.MapPost("/{userId}/override", async (
            int userId,
            AccessRequestDto dto,
            AccessService access) =>
        {
            var result = await access.RequestOverrideAsync(userId, dto);
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/complete",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var result = await access.CompleteOverrideAsync(requestId, dto);
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/approve",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var result = await access.ApproveOverrideAsync(requestId, dto.UserId);
            return ToHttpResult(result);
        });


        group.MapPost(
            "/requests/{requestId}/reject",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var result = await access.RejectOverrideAsync(requestId);
            return ToHttpResult(result);
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
                Results.Unauthorized(),

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