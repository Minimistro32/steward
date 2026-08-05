using Microsoft.Extensions.Options;
using System.Buffers;
using MQTTnet;
using Steward.Messaging;

namespace Steward.Server.Mqtt;

public class MqttConnectionService : BackgroundService, IMqttConnectionService
{
    private readonly IMqttClient mqttClient;
    private readonly ILogger<MqttConnectionService> logger;
    private readonly MqttOptions options;
    private readonly MqttMessageDispatcher messageDispatcher;

    public MqttConnectionService(
        ILogger<MqttConnectionService> logger,
        IOptions<MqttOptions> options,
        MqttMessageDispatcher messageDispatcher)
    {
        this.options = options.Value;
        this.logger = logger;
        this.messageDispatcher = messageDispatcher;

        var factory = new MqttClientFactory();
        mqttClient = factory.CreateMqttClient();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            await this.messageDispatcher.HandleAsync(
                e.ApplicationMessage.Topic,
                e.ApplicationMessage.Payload.ToArray());
        };
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Host, options.Port)
            .WithClientId(options.ClientId)
            .Build();

        // Connection
        logger.LogInformation(
            "Connecting to MQTT broker at {Host}:{Port}...",
            options.Host,
            options.Port
        );

        await mqttClient.ConnectAsync(clientOptions, stoppingToken);

        logger.LogInformation(
            "Connected to MQTT broker at {Host}:{Port}.",
            options.Host,
            options.Port
        );

        // Subscriptions
        await mqttClient.SubscribeAsync(MqttTopics.AgentRegister, cancellationToken: stoppingToken);
        await mqttClient.SubscribeAsync(MqttTopics.AgentStatusWildcard, cancellationToken: stoppingToken);
        await mqttClient.SubscribeAsync(MqttTopics.AccessResponseWildcard, cancellationToken: stoppingToken);

        logger.LogInformation("MQTT subscriptions established.");

        // Refresh Agents
        await PublishRefreshRequestAsync(stoppingToken);

        // Keep alive
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public async Task PublishRefreshRequestAsync(
    CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing agent refresh request.");

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.AgentRefresh)
            .Build();

        await mqttClient.PublishAsync(message, cancellationToken);
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