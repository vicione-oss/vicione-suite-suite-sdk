using Sdk.Connections.Contracts;

namespace Sdk.Connections.Extensions;

/// <summary>
/// Provides extension methods for <see cref="MqttConnection"/>.
/// </summary>
public static class MqttConnectionExtensions
{
    /// <summary>
    /// Generates a WebSocket URI from the given MQTT connection details.
    /// </summary>
    public static Uri GetWebsocketUri(this MqttConnection mqttConnection)
    {
        var uriBuilder = new UriBuilder(mqttConnection.Address)
        {
            Port = mqttConnection.Port
        };

        if (uriBuilder.Scheme == "http")
            uriBuilder.Scheme = "ws";
        return uriBuilder.Uri;
    }
}
