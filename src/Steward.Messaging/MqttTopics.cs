namespace Steward.Messaging;

public static class MqttTopics
{
    private const string Root = "steward/agents";
    public const string AgentRegister = $"{Root}/register";
    public const string AgentRefresh = $"{Root}/refresh";
    public const string AgentStatusWildcard = $"{Root}/+/status";
    public const string AccessResponseWildcard = $"{Root}/+/response";

    public static string AgentStatus(string agentId)
        => AgentStatusWildcard.Replace("+", agentId);

    public static string AccessRequest(string agentId)
        => $"{Root}/{agentId}/request";

    public static string AccessResponse(string agentId)
        => AccessResponseWildcard.Replace("+", agentId);

    public static bool IsAgentStatus(string topic)
        => topic.StartsWith(Root)
        && topic.EndsWith("/status");

    public static bool IsAccessResponse(string topic)
        => topic.StartsWith(Root)
        && topic.EndsWith("/response");
}