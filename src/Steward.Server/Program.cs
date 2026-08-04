using Steward.Server.Mqtt;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StewardDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("Steward")
    ));

builder.Services.Configure<MqttOptions>(
    builder.Configuration.GetSection(MqttOptions.SectionName)
);

// Add services to the container.
builder.Services.AddSingleton<MqttMessageHandler>();
builder.Services.AddHostedService<MqttConnectionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapWardEndpoints();

app.Run();