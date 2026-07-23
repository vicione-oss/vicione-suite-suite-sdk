namespace Sdk.Connections.Contracts;

/// <summary>
/// Specifies the TLS protocol version used to secure an MQTT connection.
/// </summary>
/// <remarks>
/// Only modern, secure TLS versions are exposed. Legacy protocols such as SSL 2.0, SSL 3.0,
/// TLS 1.0, and TLS 1.1 are intentionally omitted because they are considered insecure.
/// </remarks>
public enum MqttSslProtocol
{
    /// <summary>
    /// TLS 1.2: A widely supported and secure transport layer protocol.
    /// Suitable when the broker or network infrastructure does not yet support TLS 1.3.
    /// </summary>
    Tls12,

    /// <summary>
    /// TLS 1.3: The latest version of the TLS protocol, offering improved security and performance
    /// over earlier versions. Use this when both the client and the broker support it.
    /// </summary>
    Tls13
}
