using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Api.Models;
using Steward.Server.Application;

namespace Steward.Server.Api;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/", async (StewardDbContext db) =>
        {
            var users = await db.Users
                .AsSplitQuery()
                .Include(u => u.UserDevices)
                    .ThenInclude(ud => ud.Device)
                .ToListAsync();

            return Results.Ok(
                users.Select(UserDto.FromEntity)
            );
        });


        group.MapGet("/{id}", async (
            int id,
            StewardDbContext db) =>
        {
            var user = await LoadUser(db, id);

            if (user is null)
                return Results.NotFound();

            return Results.Ok(UserDto.FromEntity(user));
        });

        group.MapGet("/{id}/access", async (
            int id,
            AccessService access) =>
        {
            var response = await access.GetAccessAsync(id);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });

        /*
        {
            "options": [
                {
                "grantedResources": [
                    {
                    "id": 17,
                    "name": "PlayStation 5"
                    },
                    {
                    "id": 18,
                    "name": "Nintendo Switch"
                    },
                    {
                    "id": 19,
                    "name": "Steam"
                    }
                ],
                "devices": [
                    {
                    "id": 3,
                    "name": "Living Room Console"
                    },
                    {
                    "id": 4,
                    "name": "Gaming PC"
                    }
                ],
                "blocked": true,
                "allowedUntil": null,
                "maxRequestMinutes": 20,
                "unlocksRemaining": 4
                },
                {
                "grantedResources": [
                    {
                    "id": 42,
                    "name": "YouTube"
                    }
                ],
                "devices": [
                    {
                    "id": 7,
                    "name": "Kids Tablet"
                    }
                ],
                "blocked": false,
                "allowedUntil": "2026-08-05T17:00:00Z",
                "maxRequestMinutes": 10,
                "unlocksRemaining": 1
                }
            ]
        }
        */

        group.MapPost("/", async (
            UserDto dto,
            StewardDbContext db) =>
        {
            var user = new UserEntity
            {
                Id = dto.Id,
                Name = dto.Name
            };

            AddDevices(user, dto);


            db.Users.Add(user);

            await db.SaveChangesAsync();


            return Results.Created(
                $"/api/users/{user.Id}",
                UserDto.FromEntity(user)
            );
        });


        group.MapPut("/{id}", async (
            int id,
            UserDto dto,
            StewardDbContext db) =>
        {
            var user = await LoadUser(db, id);

            if (user is null)
                return Results.NotFound();


            user.Name = dto.Name;

            user.UserDevices.Clear();

            AddDevices(user, dto);


            await db.SaveChangesAsync();


            return Results.Ok(
                UserDto.FromEntity(user)
            );
        });

        group.MapPut("/{id}/devices/{deviceId}", async (
            int id,
            int deviceId,
            StewardDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.UserDevices)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return Results.NotFound();


            var exists = user.UserDevices
                .Any(ud => ud.DeviceId == deviceId);

            if (exists)
                return Results.NoContent();


            user.UserDevices.Add(new UserDeviceEntity
            {
                UserId = id,
                DeviceId = deviceId
            });


            await db.SaveChangesAsync();


            return Results.NoContent();
        });


        group.MapDelete("/{id}/devices/{deviceId}", async (
            int id,
            int deviceId,
            StewardDbContext db) =>
        {
            var userDevice = await db.UserDevices
                .FirstOrDefaultAsync(ud =>
                    ud.UserId == id &&
                    ud.DeviceId == deviceId);

            if (userDevice is null)
                return Results.NotFound();


            db.UserDevices.Remove(userDevice);

            await db.SaveChangesAsync();


            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (
            string id,
            StewardDbContext db) =>
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);


            if (user is null)
                return Results.NotFound();


            db.Users.Remove(user);

            await db.SaveChangesAsync();


            return Results.NoContent();
        });
    }


    private static async Task<UserEntity?> LoadUser(
        StewardDbContext db,
        int id)
    {
        return await db.Users
            .AsSplitQuery()
            .Include(u => u.UserDevices)
                .ThenInclude(ud => ud.Device)
            .FirstOrDefaultAsync(u => u.Id == id);
    }


    private static void AddDevices(
        UserEntity user,
        UserDto dto)
    {
        foreach (var deviceId in dto.DeviceIds)
        {
            user.UserDevices.Add(new UserDeviceEntity
            {
                UserId = user.Id,
                DeviceId = deviceId
            });
        }
    }
}