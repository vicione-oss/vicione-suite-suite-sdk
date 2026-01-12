using Sdk.Connections.Contracts;
using Sdk.Connections.Extensions;

namespace Sdk.Testing.Factories;

public static class ConnectionFactory
{
    public static Connection CreateMqttServiceConnection(string name, string address = "localhost/mqtt", int port = 1883, MqttConnectionType protocol = MqttConnectionType.TCP, Tag[]? tags = null)
    {
        var connection = new Connection
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = ConnectionType.Mqtt,
        };

        if (tags is not null)
        {
            foreach (var tag in tags)
                connection.Tags.Add(tag);
        }

        connection.SetMqttConnection(new MqttConnection
        {
            Address = address,
            Port = port,
            Protocol = protocol,
        });
        return connection;
    }
}
