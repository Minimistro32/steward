using MQTTnet;

namespace Steward.Server.Mqtt;

public class MqttConnectionService : BackgroundService
{
    private readonly IMqttClient mqttClient;

    public MqttConnectionService()
    {
        var factory = new MqttClientFactory();
        mqttClient = factory.CreateMqttClient();
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("steward-server")
            .Build();

        await mqttClient.ConnectAsync(options, stoppingToken);

        Console.WriteLine("Connected to MQTT broker");

        await mqttClient.PublishStringAsync(
            "steward/server/status",
            "online",
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}