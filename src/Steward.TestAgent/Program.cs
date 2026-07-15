using Steward.TestAgent.Mqtt;

var options = new AgentOptions();

var client = new AgentMqttClient(options);

await client.StartAsync();

Console.WriteLine("Press Ctrl+C to exit.");

await Task.Delay(Timeout.Infinite);