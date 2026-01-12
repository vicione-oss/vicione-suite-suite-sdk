using Sdk.Connections.Contracts;

namespace Sdk.Connections.Extensions;

public static class MqttConnectionExtensions
{
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
