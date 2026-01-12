using System.Text.Json;
using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Connection"/> to simplify common operations.
/// </summary>
public static class ConnectionExtensions
{
    /// <summary>
    /// Finds all connections in a sequence that are associated with a specific instance ID.
    /// </summary>
    public static IEnumerable<Connection> FindInstanceConnections(this IEnumerable<Connection> connections, Guid instanceId)
        => connections.Where(c
            => c.Metadata.ContainsKey(ConnectionConstants.MetaDataKeys.InstanceId)
               && string.Equals(instanceId.ToString(),
                   c.Metadata[ConnectionConstants.MetaDataKeys.InstanceId],
                   StringComparison.Ordinal));

    /// <summary>
    /// Finds the first MQTT connection in a sequence that matches a specific instance ID and MQTT connection type.
    /// </summary>
    public static Connection? FirstOrDefaultInstanceMqttConnection(this IEnumerable<Connection> connections,
        Guid instanceId, MqttConnectionType connectionType)
        => connections.FirstOrDefault(
            connection =>
            {
                if (connection.Metadata.TryGetValue(ConnectionConstants.MetaDataKeys.InstanceId, out var connectionInstanceId))
                {
                    if (!string.Equals(instanceId.ToString(), connectionInstanceId, StringComparison.Ordinal))
                        return false;

                    if (connection.Metadata.TryGetValue(ConnectionConstants.MetaDataKeys.MqttClientProtocol, out var connectionMqttClientProtocoll))
                    {
                        if (string.Equals(Enum.GetName(connectionType), connectionMqttClientProtocoll, StringComparison.Ordinal))
                            return true;
                    }
                }

                return false;
            });

    /// <summary>
    /// Sets the connection's data to a <see cref="SQLiteConnection"/> by serializing it to JSON and updating the type.
    /// </summary>
    public static void SetSQLiteConnection(this Connection connection, SQLiteConnection dbConnection)
    {
        connection.SetJson(dbConnection);
        connection.Type = ConnectionType.SQLite;
    }

    /// <summary>
    /// Sets the connection's data to a <see cref="PostgresConnection"/> by serializing it to JSON and updating the type.
    /// </summary>
    public static void SetPostgresConnection(this Connection connection, PostgresConnection dbConnection)
    {
        connection.SetJson(dbConnection);
        connection.Type = ConnectionType.Postgres;
    }

    /// <summary>
    /// Sets the connection's data to an <see cref="HttpConnection"/> by serializing it to JSON and updating the type.
    /// </summary>
    public static void SetHttpConnection(this Connection connection, HttpConnection httpConnection)
    {
        connection.SetJson(httpConnection);
        connection.Type = ConnectionType.Http;
    }

    /// <summary>
    /// Sets the connection's data to an <see cref="MqttConnection"/> by serializing it to JSON and updating the type.
    /// </summary>
    public static void SetMqttConnection(this Connection connection, MqttConnection mqttConnection)
    {
        connection.SetJson(mqttConnection);
        connection.Type = ConnectionType.Mqtt;
    }

    /// <summary>
    /// Gets the strongly-typed <see cref="SQLiteConnection"/> details if the connection type is <see cref="ConnectionType.SQLite"/>.
    /// </summary>
    public static SQLiteConnection? GetSQLiteConnection(this Connection connection)
        => connection.Type == ConnectionType.SQLite
            ? connection.GetInternal<SQLiteConnection>()
            : null;

    /// <summary>
    /// Gets the strongly-typed <see cref="PostgresConnection"/> details if the connection type is <see cref="ConnectionType.Postgres"/>.
    /// </summary>
    public static PostgresConnection? GetPostgresConnection(this Connection connection)
        => connection.Type == ConnectionType.Postgres
            ? connection.GetInternal<PostgresConnection>()
            : null;

    /// <summary>
    /// Gets the strongly-typed <see cref="HttpConnection"/> details if the connection type is <see cref="ConnectionType.Http"/>.
    /// </summary>
    public static HttpConnection? GetHttpConnection(this Connection connection)
        => connection.Type == ConnectionType.Http
            ? connection.GetInternal<HttpConnection>()
            : null;

    /// <summary>
    /// Gets the strongly-typed <see cref="MqttConnection"/> details if the connection type is <see cref="ConnectionType.Mqtt"/>.
    /// </summary>
    public static MqttConnection? GetMqttConnection(this Connection connection)
        => connection.Type == ConnectionType.Mqtt
            ? connection.GetInternal<MqttConnection>()
            : null;

    /// <summary>
    /// Deserializes the JSON content of a connection into a specified connection type.
    /// </summary>
    private static TConnection? GetInternal<TConnection>(this Connection connection)
        where TConnection : class
    {
        if (string.IsNullOrWhiteSpace(connection.Json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<TConnection>(connection.Json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Serializes a connection object to JSON and sets it on the connection's Json property.
    /// </summary>
    private static void SetJson<TConnection>(this Connection connection, TConnection obj)
        => connection.Json = JsonSerializer.Serialize(obj, DefaultJsonSerializerSettings.Default);

    /// <summary>
    /// Copies all property values from another connection object to this one.
    /// </summary>
    public static void Assign(this Connection connection, Connection other)
    {
        connection.Json = other.Json;
        connection.Description = other.Description;
        connection.Name = other.Name;
        connection.Type = other.Type;
        connection.Tags = [.. other.Tags];
        connection.Metadata = new Dictionary<string, string?>(other.Metadata);
        connection.Managed = other.Managed;
    }
}
