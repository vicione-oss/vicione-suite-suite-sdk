using System.Text.Json;
using Sdk.Connections.Contracts;

namespace Sdk.Connections;

public sealed class DefaultJsonConnectionSerializer<TConnection> : IConnectionSerializer
    where TConnection : IConnection
{
    public string Serialize(IConnection connection)
        => JsonSerializer.Serialize(connection, connection.GetType());

    public IConnection? Deserialize(string serializedConnection)
        => JsonSerializer.Deserialize<TConnection>(serializedConnection);
}
