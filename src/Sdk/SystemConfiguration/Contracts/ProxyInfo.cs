namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the configuration of a single proxy endpoint.
/// </summary>
/// <remarks>
/// The presence of a <see cref="ProxyInfo"/> instance indicates that the proxy is enabled.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record ProxyInfo
{
    /// <summary>
    /// The proxy server hostname or IP address.
    /// </summary>
    public required string Server { get; init; }

    /// <summary>
    /// The proxy server port.
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// The username for proxy authentication, or <see langword="null"/> if not configured.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// The password for proxy authentication, or <see langword="null"/> if not configured.
    /// </summary>
    public string? Password { get; init; }
}

