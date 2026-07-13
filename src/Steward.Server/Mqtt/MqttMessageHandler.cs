using System.Text;
using Steward.Messaging;
using Steward.Messaging.Messages.Agent;

namespace Steward.Server.Mqtt;

public class MqttMessageHandler
{
    private readonly ILogger<MqttMessageHandler> logger;

    public MqttMessageHandler(
        ILogger<MqttMessageHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleAsync(
        string topic,
        byte[] payload)
    {
        logger.LogInformation(
            "Received MQTT message on topic {Topic}",
            topic);

        var json = Encoding.UTF8.GetString(payload);
        logger.LogInformation(
            "Payload: {Payload}",
            json);

        if (topic == MqttTopics.AgentRegister)
        {
            HandleRegistration(payload);
        }
        else
        {
            logger.LogWarning(
                "Topic mismatch: received '{Received}'.",
                topic);
        }

        return Task.CompletedTask;
    }

    private void HandleRegistration(byte[] payload)
    {
        try
        {
            var json = Encoding.UTF8.GetString(payload);
            var registration = StewardMessage.Deserialize<RegistrationMessage>(json);

            if (registration is null)
            {
                logger.LogWarning(
                    "Failed to deserialize agent registration.");
                return;
            }

            logger.LogInformation(
                "Agent registered: {AgentId} ({Name}) with {ResourceCount} resources.",
                registration.AgentId,
                registration.Name,
                registration.Resources.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Exception while handling agent registration.");
        }
    }
}