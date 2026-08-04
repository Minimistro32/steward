namespace Steward.Server.Mqtt;

public interface IMqttConnectionService
{
    Task PublishRefreshRequestAsync(
        CancellationToken cancellationToken = default);
}