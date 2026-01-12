using System.Text.Json;
using Sdk.Connections.Contracts;

namespace Sdk.Connections;

/// <summary>
/// Provides a default, generic implementation of <see cref="IConnectionSerializer"/> using <see cref="System.Text.Json"/>.
/// </summary>
public sealed class DefaultJsonConnectionSerializer<TConnection> : IConnectionSerializer
    where TConnection : IConnection
{
    /// <summary>
    /// Serializes the specified connection object into a JSON string.
    /// </summary>
    public string Serialize(IConnection connection)
        => JsonSerializer.Serialize(connection, connection.GetType());

    /// <summary>
    /// Deserializes a JSON string into a connection object of type <typeparamref name="TConnection"/>.
    /// </summary>
    public IConnection? Deserialize(string serializedConnection)
        => JsonSerializer.Deserialize<TConnection>(serializedConnection);
}
