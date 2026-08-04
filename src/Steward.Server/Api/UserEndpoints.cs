using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Models;

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
            string id,
            StewardDbContext db) =>
        {
            var user = await LoadUser(db, id);

            if (user is null)
                return Results.NotFound();

            return Results.Ok(UserDto.FromEntity(user));
        });


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
            string id,
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
        string id)
    {
        return await db.Users
            .AsSplitQuery()
            .Include(u => u.UserDevices)
                .ThenInclude(ud => ud.Device)
            .FirstOrDefaultAsync(u => u.Id.ToString() == id);
    }


    private static void AddDevices(
        UserEntity user,
        UserDto dto)
    {
        foreach (var selection in dto.AgentSelections)
        {
            foreach (var deviceId in selection.Value.DeviceIds)
            {
                user.UserDevices.Add(new UserDeviceEntity
                {
                    UserId = user.Id,
                    DeviceId = deviceId
                });
            }
        }
    }
}