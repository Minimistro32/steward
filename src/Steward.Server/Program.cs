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
            .AllowAnyMethod();
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

// app.UseHttpsRedirection();

app.MapAccessEndpoints();
app.MapAgentEndpoints();
app.MapPolicyEndpoints();
app.MapUserEndpoints();
app.MapWardEndpoints();

app.Run();