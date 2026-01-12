namespace Sdk.Connections.Contracts;

/// <summary>
/// Specifies the transport protocol used for an MQTT connection.
/// </summary>
public enum MqttConnectionType
{
    /// <summary>
    /// The connection is established over a standard, unencrypted TCP socket.
    /// </summary>
    TCP,

    /// <summary>
    /// The connection is established over a TCP socket secured with Transport Layer Security (TLS).
    /// </summary>
    TCPWithTLS,

    /// <summary>
    /// The connection is established using the WebSocket protocol, which may be unsecured or secured with TLS.
    /// </summary>
    WebSocket
}
