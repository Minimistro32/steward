using System.Net.Mail;
using Steward.Server.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Api.Models;

namespace Steward.Server.Api;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").RequireAuthorization("Admin");

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
            StewardDbContext db, HttpContext context) =>
        {
            var user = await LoadUser(db, id);

            if (user is null)
                return Results.NotFound();
            if (!CanEdit(user, context)) return Results.Forbid();

            return Results.Ok(UserDto.FromEntity(user));
        });

        group.MapPost("/", async (SaveUserDto dto, StewardDbContext db) =>
        {
            var errors = Validate(dto, false);
            if (errors.Count > 0) return Results.ValidationProblem(errors);
            var user = new UserEntity();
            Apply(user, dto);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", UserDto.FromEntity(user));
        });

        group.MapPut("/{id}", async (int id, SaveUserDto dto, StewardDbContext db, HttpContext context) =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            var user = await LoadUser(db, id);
            if (user is null) return Results.NotFound();
            if (!CanEdit(user, context)) return Results.Forbid();
            var errors = Validate(dto, user.PinHash is not null);
            if (user.Type == UserType.Admin && dto.Type != UserType.Admin
                && !await db.Users.AnyAsync(u => u.Id != id && u.Type == UserType.Admin))
                errors["type"] = ["The last admin must remain an admin."];
            if (errors.Count > 0) return Results.ValidationProblem(errors);
            Apply(user, dto);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return Results.Ok(UserDto.FromEntity(user));
        });


        group.MapDelete("/{id}", async (
            string id,
            StewardDbContext db, HttpContext context) =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == id);


            if (user is null)
                return Results.NotFound();
            if (!CanEdit(user, context)) return Results.Forbid();


            if (user.Type == UserType.Admin && !await db.Users.AnyAsync(u => u.Id != user.Id && u.Type == UserType.Admin))
                return Results.Conflict(new { message = "The last admin cannot be deleted." });
            db.Users.Remove(user);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();


            return Results.NoContent();
        });

        group.MapPut("/{id}/devices/{deviceId}", async (
            int id,
            int deviceId,
            StewardDbContext db, HttpContext context) =>
        {
            var user = await db.Users
                .Include(u => u.UserDevices)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return Results.NotFound();
            if (!CanEdit(user, context)) return Results.Forbid();


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
            StewardDbContext db, HttpContext context) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();
            if (!CanEdit(user, context)) return Results.Forbid();
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
    }


    private static bool CanEdit(UserEntity user, HttpContext context) =>
        user.Type != UserType.Admin || user.Id == context.User.UserId();

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


    private static Dictionary<string, string[]> Validate(SaveUserDto dto, bool hasPin)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name)) errors["name"] = ["Name is required."];
        if (!Enum.IsDefined(dto.Type)) errors["type"] = ["Choose Admin or Member."];
        var email = dto.Email?.Trim();
        if (dto.Type == UserType.Admin && string.IsNullOrEmpty(email)) errors["email"] = ["Admins require an email address."];
        else if (!string.IsNullOrEmpty(email) && (!MailAddress.TryCreate(email, out var address) || address.Address != email))
            errors["email"] = ["Enter a valid email address."];
        if (!string.IsNullOrEmpty(dto.Pin) && (dto.Pin.Length < 4 || dto.Pin.Length > 128 || dto.Pin.Any(c => c is < '0' or > '9')))
            errors["pin"] = ["PIN must contain at least four digits (maximum 128)."];
        if (dto.ClearPin && !string.IsNullOrEmpty(dto.Pin)) errors["pin"] = ["Choose a new PIN or clear it, not both."];
        if (dto.Type == UserType.Admin && (dto.ClearPin || (!hasPin && string.IsNullOrEmpty(dto.Pin))))
            errors["pin"] = ["Admins require a PIN."];
        return errors;
    }

    private static void Apply(UserEntity user, SaveUserDto dto)
    {
        if (dto.ClearPin || !string.IsNullOrEmpty(dto.Pin) || user.Type != dto.Type
            || !string.Equals(user.Email, dto.Email?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            user.RecoveryPinHash = null;
            user.RecoveryPinExpiresAt = null;
        }
        user.Name = dto.Name.Trim();
        user.Type = dto.Type;
        user.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        if (dto.ClearPin) user.PinHash = null;
        else if (!string.IsNullOrEmpty(dto.Pin)) user.PinHash = new PasswordHasher<UserEntity>().HashPassword(user, dto.Pin);
    }
}
