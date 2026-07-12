namespace Steward.Messaging.Messages.Agent;

public sealed class ResponseMessage
{
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