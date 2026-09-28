using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Authentication;
using Steward.Server.Data;
using Steward.Server.Data.Entities;

namespace Steward.Server.Api;

public static class AuthEndpoints
{
    public sealed record SignInRequest(int UserId, string? Pin);
    private static object Identity(UserEntity user) => new { user.Id, user.Name, user.Type };

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");
        group.MapGet("/users", async (StewardDbContext db) =>
            Results.Ok(await db.Users.OrderBy(user => user.Name).Select(user => new { user.Id, user.Name }).ToListAsync())).AllowAnonymous();

        group.MapPost("/login", async (SignInRequest input, StewardDbContext db, HttpContext context) =>
        {
            var user = await db.Users.FindAsync(input.UserId);
            var pin = input.Pin ?? "";
            if (user is null || pin.Length > 128 || pin.Any(c => c is < '0' or > '9')) return Results.Unauthorized();
            var hasher = new PasswordHasher<UserEntity>();
            var result = PasswordVerificationResult.Failed;
            if (user.PinHash is null)
            {
                if (user.Type == UserType.Member && pin.Length == 0) result = PasswordVerificationResult.Success;
            }
            else
            {
                try { result = hasher.VerifyHashedPassword(user, user.PinHash, pin); }
                catch (FormatException) { return Results.Unauthorized(); }
            }
            if (result == PasswordVerificationResult.Failed) return Results.Unauthorized();
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PinHash = hasher.HashPassword(user, pin);
                await db.SaveChangesAsync();
            }
            await context.SignOutAsync();
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
                new Claim("credential", SessionAuthentication.CredentialStamp(user))
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = false });
            return Results.Ok(Identity(user));
        }).AllowAnonymous().RequireRateLimiting("login");

        group.MapGet("/session", async (StewardDbContext db, HttpContext context) =>
            Results.Ok(Identity((await db.Users.FindAsync(context.User.UserId()))!))).RequireAuthorization();
        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
