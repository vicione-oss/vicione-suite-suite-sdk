using System.Text.Json;
using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Extensions;

public static class ConnectionExtensions
{
    public static IEnumerable<Connection> FindInstanceConnections(this IEnumerable<Connection> connections, Guid instanceId)
        => connections.Where(c
            => c.Metadata.ContainsKey(ConnectionConstants.MetaDataKeys.InstanceId)
               && string.Equals(instanceId.ToString(),
                   c.Metadata[ConnectionConstants.MetaDataKeys.InstanceId],
                   StringComparison.Ordinal));

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

    public static void SetAzureIotHubConnection(this Connection connection, AzureIotHubConnection azureHubConnection)
    {
        connection.SetJson(azureHubConnection);
        connection.Type = ConnectionType.AzureIotHub;
    }

    public static void SetDatabaseConnection(this Connection connection, DatabaseConnection dbConnection)
    {
        connection.SetJson(dbConnection);
        connection.Type = ConnectionType.Database;
    }

    public static void SetHttpConnection(this Connection connection, HttpConnection httpConnection)
    {
        connection.SetJson(httpConnection);
        connection.Type = ConnectionType.Http;
    }

    public static void SetMqttConnection(this Connection connection, MqttConnection mqttConnection)
    {
        connection.SetJson(mqttConnection);
        connection.Type = ConnectionType.Mqtt;
    }

    public static AzureIotHubConnection? GetAzureIotHubConnection(this Connection connection)
        => connection.Type == ConnectionType.AzureIotHub
            ? connection.GetInternal<AzureIotHubConnection>()
            : null;

    public static DatabaseConnection? GetDatabaseConnection(this Connection connection)
        => connection.Type == ConnectionType.Database
            ? connection.GetInternal<DatabaseConnection>()
            : null;

    public static HttpConnection? GetHttpConnection(this Connection connection)
        => connection.Type == ConnectionType.Http
            ? connection.GetInternal<HttpConnection>()
            : null;

    public static MqttConnection? GetMqttConnection(this Connection connection)
        => connection.Type == ConnectionType.Mqtt
            ? connection.GetInternal<MqttConnection>()
            : null;

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

    private static void SetJson<TConnection>(this Connection connection, TConnection obj)
        => connection.Json = JsonSerializer.Serialize(obj, DefaultJsonSerializerSettings.Default);

    public static void Assign(this Connection connection, Connection other)
    {
        connection.Json = other.Json;
        connection.Description = other.Description;
        connection.Name = other.Name;
        connection.Type = other.Type;
        connection.Tags = [.. other.Tags];
        connection.Metadata = new(other.Metadata);
    }
}
