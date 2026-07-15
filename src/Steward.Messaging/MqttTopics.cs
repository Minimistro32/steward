namespace Steward.Messaging;

public static class MqttTopics
{
    private const string Root = "steward/agents";
    public const string AgentRegister = $"{Root}/register";
    public const string AgentRefresh = $"{Root}/refresh";
    public const string AgentStatusWildcard = $"{Root}/+/status";
    public const string AgentResponseWildcard = $"{Root}/+/response";

    public static string StewardCommand(string agentId)
        => $"{Root}/{agentId}/command";

    public static string AgentStatus(string agentId)
        => AgentStatusWildcard.Replace("+", agentId);

    public static string AgentResponse(string agentId)
        => AgentResponseWildcard.Replace("+", agentId);

    public static bool IsAgentStatus(string topic)
        => topic.StartsWith(Root)
        && topic.EndsWith("/status");

    public static bool IsAgentResponse(string topic)
        => topic.StartsWith(Root)
        && topic.EndsWith("/response");
}