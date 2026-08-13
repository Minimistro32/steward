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
            var response = await access.RequestAccessAsync(userId, dto);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost("/{userId}/override", async (
            int userId,
            AccessRequestDto dto,
            AccessService access) =>
        {
            var response = await access.RequestOverrideAsync(userId, dto);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost(
            "/requests/{requestId}/complete",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var response =
                await access.CompleteOverrideAsync(requestId, dto);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost(
            "/requests/{requestId}/approve",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var response =
                await access.ApproveOverrideAsync(requestId, dto.UserId);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });


        group.MapPost(
            "/requests/{requestId}/reject",
            async (
                int requestId,
                OverrideActionDto dto,
                AccessService access) =>
        {
            var response =
                await access.RejectOverrideAsync(requestId);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });
    }
}