using System.Text.Json;
using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Connection"/> to simplify common operations.
/// </summary>
public static class ConnectionExtensions
{
    extension(IEnumerable<Connection> connections)
    {
        /// <summary>
        /// Finds all connections in a sequence that are associated with a specific instance ID.
        /// </summary>
        public IEnumerable<Connection> FindInstanceConnections(Guid instanceId)
            => connections.Where(c
                => c.Metadata.ContainsKey(ConnectionConstants.MetaDataKeys.InstanceId)
                   && string.Equals(instanceId.ToString(),
                       c.Metadata[ConnectionConstants.MetaDataKeys.InstanceId],
                       StringComparison.Ordinal));

        /// <summary>
        /// Finds the first MQTT connection in a sequence that matches a specific instance ID and MQTT connection type.
        /// </summary>
        public Connection? FirstOrDefaultInstanceMqttConnection(Guid instanceId, MqttConnectionType connectionType)
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
    }

    extension(Connection connection)
    {
        /// <summary>
        /// Sets the connection's data to a <see cref="SQLiteConnection"/> by serializing it to JSON and updating the type.
        /// </summary>
        public void SetSQLiteConnection(SQLiteConnection dbConnection)
        {
            connection.SetJson(dbConnection);
            connection.Type = ConnectionType.SQLite;
        }

        /// <summary>
        /// Sets the connection's data to a <see cref="PostgresConnection"/> by serializing it to JSON and updating the type.
        /// </summary>
        public void SetPostgresConnection(PostgresConnection dbConnection)
        {
            connection.SetJson(dbConnection);
            connection.Type = ConnectionType.Postgres;
        }

        /// <summary>
        /// Sets the connection's data to an <see cref="HttpConnection"/> by serializing it to JSON and updating the type.
        /// </summary>
        public void SetHttpConnection(HttpConnection httpConnection)
        {
            connection.SetJson(httpConnection);
            connection.Type = ConnectionType.Http;
        }

        /// <summary>
        /// Sets the connection's data to an <see cref="MqttConnection"/> by serializing it to JSON and updating the type.
        /// </summary>
        public void SetMqttConnection(MqttConnection mqttConnection)
        {
            connection.SetJson(mqttConnection);
            connection.Type = ConnectionType.Mqtt;
        }

        /// <summary>
        /// Returns the <see cref="SQLiteConnection"/> details if the type is <see cref="ConnectionType.SQLite"/>;
        /// otherwise <see langword="null"/>.
        /// </summary>
        public SQLiteConnection? GetSQLiteConnection()
            => connection.Type == ConnectionType.SQLite
                ? connection.GetInternal<SQLiteConnection>()
                : null;

        /// <summary>
        /// Returns the <see cref="PostgresConnection"/> details if the type is <see cref="ConnectionType.Postgres"/>;
        /// otherwise <see langword="null"/>.
        /// </summary>
        public PostgresConnection? GetPostgresConnection()
            => connection.Type == ConnectionType.Postgres
                ? connection.GetInternal<PostgresConnection>()
                : null;

        /// <summary>
        /// Returns the <see cref="HttpConnection"/> details if the type is <see cref="ConnectionType.Http"/>;
        /// otherwise <see langword="null"/>.
        /// </summary>
        public HttpConnection? GetHttpConnection()
            => connection.Type == ConnectionType.Http
                ? connection.GetInternal<HttpConnection>()
                : null;

        /// <summary>
        /// Returns the <see cref="MqttConnection"/> details if the type is <see cref="ConnectionType.Mqtt"/>;
        /// otherwise <see langword="null"/>.
        /// </summary>
        public MqttConnection? GetMqttConnection()
            => connection.Type == ConnectionType.Mqtt
                ? connection.GetInternal<MqttConnection>()
                : null;

        /// <summary>
        /// Deserializes the JSON content of a connection into a specified connection type.
        /// </summary>
        private TConnection? GetInternal<TConnection>()
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
        private void SetJson<TConnection>(TConnection obj)
            => connection.Json = JsonSerializer.Serialize(obj, DefaultJsonSerializerSettings.Default);

        /// <summary>
        /// Copies all property values from another connection object to this one.
        /// </summary>
        public void Assign(Connection other)
        {
            connection.Json = other.Json;
            connection.Description = other.Description;
            connection.Name = other.Name;
            connection.Type = other.Type;
            connection.Tags = [.. other.Tags];
            connection.Metadata = other.Metadata.ToDictionary();
            connection.Managed = other.Managed;
        }
    }
}
