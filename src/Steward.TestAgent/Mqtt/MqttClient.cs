using System.Buffers;
using MQTTnet;
using Steward.Messaging;
using Steward.Messaging.Messages.Agent;

namespace Steward.TestAgent.Mqtt;

public class MqttClient
{
    private readonly IMqttClient mqttClient;
    private readonly MqttOptions options;

    public MqttClient(MqttOptions options)
    {
        this.options = options;

        var factory = new MqttClientFactory();
        mqttClient = factory.CreateMqttClient();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            await HandleMessageAsync(
                e.ApplicationMessage.Topic,
                e.ApplicationMessage.Payload.ToArray());
        };
    }

    public async Task StartAsync()
    {
        var lastWillTestament = new StatusMessage
        {
            State = AgentStatus.Offline
        };

        var clientOptions =
            new MqttClientOptionsBuilder()
                .WithTcpServer(options.Host, options.Port)
                .WithClientId(options.AgentId)
                .WithWillTopic(MqttTopics.AgentStatus(options.AgentId))
                .WithWillPayload(
                    StewardMessage.Serialize(lastWillTestament)
                )
                .WithWillRetain()
                .Build();

        await mqttClient.ConnectAsync(clientOptions);
        
        Console.WriteLine("Connected.");

        await mqttClient.SubscribeAsync(MqttTopics.AgentRefresh);
        await PublishOnlineStatusAsync();

        Console.WriteLine("Agent started.");
    }

    private async Task HandleMessageAsync(string topic, byte[] payload)
    {
        if (topic == MqttTopics.AgentRefresh)
        {
            await RegisterAsync();
        }
    }

    private async Task RegisterAsync()
    {
        var message = new RegistrationMessage
        {
            AgentId = options.AgentId,
            InstanceId = options.InstanceId,
            Name = options.Name
        };

        await mqttClient.PublishStringAsync(
            MqttTopics.AgentRegister,
            StewardMessage.Serialize(message)
        );

        Console.WriteLine("Published registration.");
    }

    private async Task PublishOnlineStatusAsync()
    {
        var message = new StatusMessage
        {
            State = AgentStatus.Online
        };

        await mqttClient.PublishAsync(
            new MqttApplicationMessageBuilder()
                .WithTopic(
                    MqttTopics.AgentStatus(options.AgentId))
                .WithPayload(
                    StewardMessage.Serialize(message)
                )
                .WithRetainFlag()
                .Build()
        );
    }
}