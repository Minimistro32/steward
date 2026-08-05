namespace Steward.Messaging.Messages;

public sealed class AccessResponseMessage
{
    /* steward/agents/test-agent/response
    {
        "requestId": "123",
        "status": "completed"
    }*/

    public required string RequestId { get; init; }

    public required AccessRequestStatus RequestStatus { get; init; }

    public string? Message { get; init; }
}

public enum AccessRequestStatus
{
    Accepted,
    Completed,
    Failed
}