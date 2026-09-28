using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace Steward.Server.Authentication;

// Server-side tickets make logout revoke the session immediately. Restarts sign everyone out.
public sealed class SessionStore(IMemoryCache cache) : ITicketStore
{
    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Guid.NewGuid().ToString("N");
        cache.Set(key, ticket, ticket.Properties.ExpiresUtc!.Value);
        return Task.FromResult(key);
    }
    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        cache.Set(key, ticket, ticket.Properties.ExpiresUtc!.Value);
        return Task.CompletedTask;
    }
    public Task<AuthenticationTicket?> RetrieveAsync(string key) =>
        Task.FromResult(cache.Get<AuthenticationTicket>(key));
    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
