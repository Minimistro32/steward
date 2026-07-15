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
            json
        );

        // switch on topic
        if (topic == MqttTopics.AgentRegister)
        {
            HandleRegistration(payload);
        }
        else if (MqttTopics.IsAgentStatus(topic))
        {
            HandleStatus(payload);
        }
        else if (MqttTopics.IsAgentResponse(topic))
        {
            HandleResponse(payload);
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
                "Exception while handling agent registration."
            );
        }
    }

    private void HandleStatus(byte[] payload)
    {
        var json = Encoding.UTF8.GetString(payload);
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

    private void HandleResponse(byte[] payload)
    {
        var json = Encoding.UTF8.GetString(payload);
        var message = StewardMessage.Deserialize<ResponseMessage>(json);

        if (message is null)
        {
            logger.LogWarning(
                "Failed to deserialize response message.");
            return;
        }

        logger.LogInformation(
            "Response {RequestId}: {Status}",
            message.RequestId,
            message.CommandStatus
        );
    }
}