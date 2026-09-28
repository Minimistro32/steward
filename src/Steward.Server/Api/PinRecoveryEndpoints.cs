using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;
using Steward.Server.Email;

namespace Steward.Server.Api;

public static class PinRecoveryEndpoints
{
    public sealed record RecoveryRequest(int UserId, string? Email);

    public static void MapPinRecoveryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/auth/recovery", (SmtpEmailSender sender) => Results.Ok(new { enabled = sender.IsConfigured })).AllowAnonymous();
        app.MapPost("/api/auth/recovery", async (RecoveryRequest input, StewardDbContext db,
            SmtpEmailSender sender, ILogger<SmtpEmailSender> logger, CancellationToken cancellationToken) =>
        {
            if (!sender.IsConfigured) return Results.Problem("Email recovery is not configured. Ask the server owner to configure SMTP.", statusCode: 503);
            // Serialize resets and account edits. Rollback preserves credentials if SMTP fails.
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user = await db.Users.FindAsync([input.UserId], cancellationToken);
            var now = DateTimeOffset.UtcNow;
            if (user is null || user.Type != UserType.Admin || string.IsNullOrWhiteSpace(user.Email)
                || !string.Equals(user.Email.Trim(), input.Email?.Trim(), StringComparison.OrdinalIgnoreCase)
                || user.LastRecoveryEmailAt > now.AddMinutes(-5))
                return Results.Accepted();

            var hasher = new PasswordHasher<UserEntity>();
            string pin;
            do { pin = RandomNumberGenerator.GetInt32(100_000_000).ToString("D8", CultureInfo.InvariantCulture); }
            while (user.PinHash is not null && hasher.VerifyHashedPassword(user, user.PinHash, pin) != PasswordVerificationResult.Failed);
            user.RecoveryPinHash = hasher.HashPassword(user, pin);
            user.RecoveryPinExpiresAt = now.AddMinutes(30);
            user.LastRecoveryEmailAt = now;
            try
            {
                // Persist only after the SMTP relay accepts the message. No PINs are logged.
                await sender.SendRecoveryPinAsync(user.Email, pin, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return Results.Accepted();
            }
            catch (Exception exception) when (exception is SmtpException or OperationCanceledException or InvalidOperationException or DbUpdateException)
            {
                logger.LogWarning("PIN recovery could not complete. Check SMTP connectivity/configuration and database availability.");
                return Results.Problem("Couldn’t send a new PIN. Your existing PIN is unchanged. Try again later or contact the server owner.", statusCode: 503);
            }
        }).AllowAnonymous().RequireRateLimiting("recovery");
    }
}
