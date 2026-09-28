using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Threading.RateLimiting;
using Steward.Server.Authentication;
using Steward.Server.Mqtt;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Api;
using Steward.Server.Application;
using System.Text.Json.Serialization;
using Steward.Server.Mqtt.Handlers;
using System.Text.Json;
using Steward.Server.Setup;

var setupRequested = args.FirstOrDefault() == "setup";
var builder = WebApplication.CreateBuilder(setupRequested ? args[1..] : args);

builder.Services.AddDbContextFactory<StewardDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Steward"))
);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SessionStore>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "steward.session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = false;
    options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = 401; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = context => { context.Response.StatusCode = 403; return Task.CompletedTask; };
    options.Events.OnValidatePrincipal = SessionAuthentication.ValidateAsync;
});
builder.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<SessionStore>((options, store) => options.SessionStore = store);
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});

builder.Services.Configure<MqttOptions>(
    builder.Configuration.GetSection(MqttOptions.SectionName)
);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Add services to the container.
// MQTT
builder.Services.AddSingleton<MqttMessageDispatcher>();
builder.Services.AddSingleton<RegistrationMessageHandler>();
builder.Services.AddSingleton<StatusMessageHandler>();

builder.Services.AddSingleton<MqttConnectionService>();
builder.Services.AddHostedService(sp =>
    sp.GetRequiredService<MqttConnectionService>());

// Application
builder.Services.AddScoped<PolicyEvaluator>();
builder.Services.AddScoped<AccessService>();

// TEMPORARY FOR DEVELOPMENT
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StewardDbContext>();
    await db.Database.MigrateAsync();
    if (setupRequested)
    {
        Environment.ExitCode = await TerminalSetup.RunAsync(db);
        return;
    }

    if (!await db.Users.AnyAsync(user => user.Type == Steward.Server.Data.Entities.UserType.Admin))
    {
        Console.Error.WriteLine(await db.SetupStates.AnyAsync()
            ? "Steward has no admin account. Restore an admin from your database backup; initial setup cannot be repeated."
            : "Steward needs an admin. Run: dotnet run --project src/Steward.Server -- setup");
        Environment.ExitCode = 1;
        return;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("frontend");
    // app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
// A custom header makes cookie-authenticated mutations require a same-origin
// request or a successful CORS preflight from an explicitly allowed origin.
app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    if (context.Request.Path.StartsWithSegments("/api")
        && !HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method)
        && !HttpMethods.IsOptions(context.Request.Method)
        && context.Request.Headers["X-Steward-Request"] != "1")
    {
        context.Response.StatusCode = 403;
        return;
    }
    await next(context);
});
app.MapAuthEndpoints();

app.MapAccessEndpoints();
app.MapAgentEndpoints();
app.MapPolicyEndpoints();
app.MapUserEndpoints();
app.MapWardEndpoints();

app.Run();