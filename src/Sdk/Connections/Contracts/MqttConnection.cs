namespace Sdk.Connections.Contracts;

public sealed record MqttConnection : IConnection
{
    public string Address { get; set; } = string.Empty;
    public MqttConnectionType Protocol { get; set; }
    public int Port { get; set; } = 1883;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Certificate
    public string? ClientCertificate { get; set; }
    public string? ClientCertificateKey { get; set; }

    // Connection Properties
    public string? ClientId { get; set; }

    public string? WillTopic { get; set; }
    public string? WillMessage { get; set; }
    public bool WillRetain { get; set; } = true;

    public bool CleanSession { get; set; } = true;
}
