using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;

namespace Steward.Server.Authentication;

public static class SessionAuthentication
{
    public static int UserId(this ClaimsPrincipal principal) => int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public static string CredentialStamp(UserEntity user) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(user.PinHash ?? "")));

    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal!;
        var db = context.HttpContext.RequestServices.GetRequiredService<StewardDbContext>();
        var user = await db.Users.FindAsync(principal.UserId());
        if (user is null || principal.FindFirstValue("credential") != CredentialStamp(user)
            || !principal.IsInRole(user.Type.ToString()))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
        }
    }
}
