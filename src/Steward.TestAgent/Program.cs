using Steward.TestAgent.Mqtt;

var options = new MqttOptions();
var client = new MqttClient(options);

await client.StartAsync();

Console.WriteLine("Press Ctrl+C to exit.");

await Task.Delay(Timeout.Infinite);