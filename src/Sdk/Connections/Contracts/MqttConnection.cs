namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents the configuration for a connection to an MQTT broker.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record MqttConnection : IConnection
{
    /// <summary>
    /// Gets or sets the network address of the MQTT broker, such as a hostname or IP address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the communication protocol for the MQTT connection. Defaults to <see cref="MqttConnectionType.TCP"/>.
    /// </summary>
    public MqttConnectionType Protocol { get; set; } = MqttConnectionType.TCP;

    /// <summary>
    /// Gets or sets the protocol version for the MQTT connection. Defaults to <see cref="MqttProtocolVersion.V500"/>.
    /// </summary>
    public MqttProtocolVersion ProtocolVersion { get; set; } = MqttProtocolVersion.V500;

    /// <summary>
    /// Gets or sets the quality of service level for the MQTT connection. Defaults to <see cref="MqttQualityOfServiceLevel.AtMostOnce"/>.
    /// </summary>
    public MqttQualityOfServiceLevel QualityOfService { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

    /// <summary>
    /// Gets or sets the TLS protocol version used to secure the MQTT connection.
    /// When <see langword="null"/>, no explicit TLS version is enforced and the system default is used.
    /// </summary>
    public MqttSslProtocol? SslProtocol { get; set; }

    /// <summary>
    /// Gets or sets the network port of the MQTT broker. The default is 1883.
    /// </summary>
    public int Port { get; set; } = 1883;

    /// <summary>
    /// Gets or sets the username for authenticating with the MQTT broker.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for authenticating with the MQTT broker.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the client certificate file for TLS-based authentication.
    /// </summary>
    public string? ClientCertificate { get; set; }

    /// <summary>
    /// Gets or sets the path to the private key file for the client certificate.
    /// </summary>
    public string? ClientCertificateKey { get; set; }

    /// <summary>
    /// Gets or sets a passphrase to be loaded if encrypted private keys need it.
    /// </summary>
    public string? ClientCertificateKeyPassword { get; set; }

    /// <summary>
    /// Gets or sets the value which determines whether the client should accept untrusted or self-signed certificates when establishing a secure connection.
    /// </summary>
    public bool AllowUntrustedCertificates { get; set; }

    /// <summary>
    /// Gets or sets the client identifier to be used when connecting to the MQTT broker.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the topic for the "Last Will and Testament" (LWT) message.
    /// </summary>
    public string? WillTopic { get; set; }

    /// <summary>
    /// Gets or sets the payload for the "Last Will and Testament" (LWT) message.
    /// </summary>
    public string? WillMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the "Last Will and Testament" (LWT) message should be retained by the broker.
    /// </summary>
    public bool WillRetain { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the client should establish a clean session with the broker.
    /// </summary>
    public bool CleanSession { get; set; } = true;

    /// <summary>
    /// Gets or sets the seconds to keep the connection alive. The default is 60 seconds.
    /// </summary>
    public int KeepAliveSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the timeout in seconds for establishing a connection to the MQTT broker. The default is 30 seconds.
    /// </summary>
    public int ConnectTimeoutSeconds { get; set; } = 30;
}
