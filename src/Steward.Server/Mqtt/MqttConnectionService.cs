using Microsoft.Extensions.Options;
using System.Buffers;
using MQTTnet;
using Steward.Messaging;
using Steward.Messaging.Messages.Steward;

namespace Steward.Server.Mqtt;

public class MqttConnectionService : BackgroundService
{
    private readonly IMqttClient mqttClient;
    private readonly ILogger<MqttConnectionService> logger;
    private readonly MqttOptions options;
    private readonly MqttMessageHandler messageHandler;

    public MqttConnectionService(
        ILogger<MqttConnectionService> logger,
        IOptions<MqttOptions> options,
        MqttMessageHandler messageHandler)
    {
        this.options = options.Value;
        this.logger = logger;
        this.messageHandler = messageHandler;

        var factory = new MqttClientFactory();
        mqttClient = factory.CreateMqttClient();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            await this.messageHandler.HandleAsync(
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
        await mqttClient.SubscribeAsync(MqttTopics.AgentResponseWildcard, cancellationToken: stoppingToken);

        // Refresh Agents
        await mqttClient.PublishStringAsync(
            MqttTopics.AgentRefresh,
            StewardMessage.Serialize(new RefreshAgentsMessage()),
            cancellationToken: stoppingToken
        );

        // Keep alive
        await Task.Delay(Timeout.Infinite, stoppingToken);
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