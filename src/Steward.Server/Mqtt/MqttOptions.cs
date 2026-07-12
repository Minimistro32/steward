namespace Steward.Server.Mqtt;

public class MqttOptions
{
    public const string SectionName = "Mqtt";
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string ClientId { get; init; } = string.Empty;
}