namespace Steward.TestAgent.Mqtt;

public class MqttOptions
{
    public string Host { get; init; } = "localhost";

    public int Port { get; init; } = 1883;

    public string AgentId { get; init; } = "test-agent";

    public string InstanceId { get; init; } = Guid.NewGuid().ToString();

    public string Name { get; init; } = "Test Agent";
}