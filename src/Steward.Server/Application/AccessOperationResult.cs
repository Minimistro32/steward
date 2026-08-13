using Steward.Server.Api.Models;

namespace Steward.Server.Application;

public enum AccessOperationStatus
{
    Success,
    NotFound,
    Unauthorized,
    Forbidden,
    Invalid,
    Conflict
}

public sealed class AccessOperationResult
{
    public required AccessOperationStatus Status { get; init; }

    public AccessResponseDto? Response { get; init; }

    public static AccessOperationResult Success(
        AccessResponseDto response)
        => new()
        {
            Status = AccessOperationStatus.Success,
            Response = response
        };

    public static AccessOperationResult NotFound()
        => new()
        {
            Status = AccessOperationStatus.NotFound
        };

    public static AccessOperationResult Unauthorized()
        => new()
        {
            Status = AccessOperationStatus.Unauthorized
        };

    public static AccessOperationResult Forbidden()
        => new()
        {
            Status = AccessOperationStatus.Forbidden
        };

    public static AccessOperationResult Invalid()
        => new()
        {
            Status = AccessOperationStatus.Invalid
        };

    public static AccessOperationResult Conflict()
        => new()
        {
            Status = AccessOperationStatus.Conflict
        };
}