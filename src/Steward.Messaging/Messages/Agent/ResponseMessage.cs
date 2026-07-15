namespace Steward.Messaging.Messages.Agent;

public sealed class ResponseMessage
{
    /* steward/agents/test-agent/response
    {
        "requestId": "123",
        "status": "completed"
    }*/

    public required string RequestId { get; init; }

    public required AgentCommandStatus CommandStatus { get; init; }

    public string? Message { get; init; }
}

public enum AgentCommandStatus
{
    Accepted,
    Completed,
    Failed
}