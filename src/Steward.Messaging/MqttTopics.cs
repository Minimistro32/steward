namespace Steward.Messaging;

public static class MqttTopics
{
    public const string AgentRegister = "steward/agents/register";

    public const string AgentRefresh = "steward/agents/refresh";

    public const string AgentStatusSubscription = "steward/agents/+/status";

    public static string AgentStatus(string agentId)
        => $"steward/agents/{agentId}/status";

    public static string AgentCommand(string agentId)
        => $"steward/agents/{agentId}/command";

    public static string AgentResponse(string agentId)
        => $"steward/agents/{agentId}/response";
}