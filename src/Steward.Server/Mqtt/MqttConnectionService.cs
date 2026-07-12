using MQTTnet;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Steward.Server.Mqtt;

public class MqttConnectionService : BackgroundService
{
    private readonly IMqttClient mqttClient;
    private readonly ILogger<MqttConnectionService> logger;
    private readonly MqttOptions options;

    public MqttConnectionService(
        ILogger<MqttConnectionService> logger,
        IOptions<MqttOptions> options)
    {
        this.options = options.Value;
        this.logger = logger;

        var factory = new MqttClientFactory();
        mqttClient = factory.CreateMqttClient();
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithClientId(options.ClientId)
            .Build();

        logger.LogInformation(
            "Connecting to MQTT broker at {Host}:{Port}...",
            options.Host,
            options.Port);

        await mqttClient.ConnectAsync(clientOptions, stoppingToken);

        logger.LogInformation(
            "Connected to MQTT broker at {Host}:{Port}.",
            options.Host,
            options.Port);

        await mqttClient.PublishStringAsync(
            "steward/server/status",
            "online",
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (mqttClient.IsConnected)
        {
            await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
        }

        logger.LogInformation("Disconnected from MQTT broker.");

        await base.StopAsync(cancellationToken);
    }
}