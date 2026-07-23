namespace Sdk.Connections.Contracts;

/// <summary>
/// Specifies the protocol version used for an MQTT connection.
/// </summary>
public enum MqttProtocolVersion
{
    /// <summary>
    /// The connection uses MQTT protocol version 3.1.1.
    /// </summary>
    V311,

    /// <summary>
    /// The connection uses MQTT protocol version 5.0.
    /// </summary>
    V500
}
