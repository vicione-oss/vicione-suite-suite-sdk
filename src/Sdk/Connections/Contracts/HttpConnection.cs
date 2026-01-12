namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents the specific configuration for an HTTP/HTTPS connection.
/// </summary>
public sealed class HttpConnection : IConnection
{
    /// <summary>
    /// Gets or sets the base address for the HTTP client (e.g. "https://api.example.com/").
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional API key used for authentication.
    /// </summary>
    public string? ApiKey { get; set; }
}
