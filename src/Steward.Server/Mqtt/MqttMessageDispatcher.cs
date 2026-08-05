using System.Text;
using Steward.Messaging;
using Steward.Messaging.Messages;
using Steward.Server.Mqtt.Handlers;

namespace Steward.Server.Mqtt;

public class MqttMessageDispatcher(
        ILogger<MqttMessageDispatcher> logger,
        RegistrationMessageHandler registrationHandler)
{
    private readonly RegistrationMessageHandler registrationHandler = registrationHandler;
    private readonly ILogger<MqttMessageDispatcher> logger = logger;

    public async Task HandleAsync(
        string topic,
        byte[] payload)
    {
        logger.LogInformation(
            "Received MQTT message on topic {Topic}",
            topic);

        var json = Encoding.UTF8.GetString(payload);

        logger.LogInformation(
            "Payload: {Payload}",
            json
        );

        // switch on topic
        if (topic == MqttTopics.AgentRegister)
        {
            await registrationHandler.HandleAsync(json);
        }
        else if (MqttTopics.IsAgentStatus(topic))
        {
            HandleStatus(json);
        }
        else if (MqttTopics.IsAccessResponse(topic))
        {
            HandleResponse(json);
        }
        else
        {
            logger.LogWarning(
                "Topic mismatch: received '{Received}'.",
                topic);
        }
    }

    private void HandleStatus(string json)
    {
        var message = StewardMessage.Deserialize<StatusMessage>(json);

        if (message is null)
        {
            logger.LogWarning(
                "Failed to deserialize status message.");
            return;
        }

        logger.LogInformation(
            "Agent status: {Status}",
            message.State
        );
    }

    private void HandleResponse(string json)
    {
        var message = StewardMessage.Deserialize<AccessResponseMessage>(json);

        if (message is null)
        {
            logger.LogWarning(
                "Failed to deserialize response message.");
            return;
        }

        logger.LogInformation(
            "Response {RequestId}: {Status}",
            message.RequestId,
            message.RequestStatus
        );
    }
}