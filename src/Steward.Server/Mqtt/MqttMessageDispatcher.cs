using System.Text;
using Steward.Messaging;
using Steward.Messaging.Messages.Agent;
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
        else if (MqttTopics.IsAgentResponse(topic))
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

    // private async void HandleRegistration(string json)
    // {
    //     try
    //     {
    //         var registration = StewardMessage.Deserialize<RegistrationMessage>(json);

    //         if (registration is null)
    //         {
    //             logger.LogWarning(
    //                 "Failed to deserialize agent registration.");
    //             return;
    //         }

    //         logger.LogInformation(
    //             "Agent registered: {AgentId} ({Name}) with {Devices} devices and {ResourceCount} resources.",
    //             registration.AgentId,
    //             registration.Name,
    //             registration.Devices.Count,
    //             registration.Resources.Count);

    //         await using var db = await dbFactory.CreateDbContextAsync();

    //         var agent = await db.Agents
    //             .Include(a => a.Devices)
    //             .Include(a => a.Resources)
    //             .SingleOrDefaultAsync(a =>
    //                 a.AgentId == registration.AgentId);

    //         if (agent is null)
    //         {
    //             agent = new Agent
    //             {
    //                 AgentId = registration.AgentId
    //             };

    //             db.Agents.Add(agent);
    //         }

    //         agent.Name = registration.Name;
    //         agent.InstanceId = registration.InstanceId;
    //         agent.Version = registration.Version;

    //         SynchronizeDevices(agent, registration);
    //         SynchronizeResources(agent, registration);

    //         await db.SaveChangesAsync();

    //         logger.LogInformation(
    //             "Registered agent {AgentId}.",
    //             agent.AgentId);
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError(
    //             ex,
    //             "Exception while handling agent registration."
    //         );
    //     }
    // }

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