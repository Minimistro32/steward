using Steward.Server.Mqtt;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Api;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StewardDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("Steward")
    ));

builder.Services.Configure<MqttOptions>(
    builder.Configuration.GetSection(MqttOptions.SectionName)
);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

// Add services to the container.
builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddHostedService<MqttConnectionService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("frontend");
    // app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapAgentEndpoints();
app.MapPolicyEndpoints();
app.MapUserEndpoints();
app.MapWardEndpoints();

app.Run();