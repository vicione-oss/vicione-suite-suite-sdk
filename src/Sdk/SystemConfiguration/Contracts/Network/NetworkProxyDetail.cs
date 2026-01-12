using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents a <see cref="NetworkProxyDetail"/>.
/// </summary>
public sealed record NetworkProxyDetail
{
    /// <summary>
    /// Represents the unconfigured <see cref="NetworkProxyDetail"/>.
    /// </summary>
    [JsonIgnore]
    public static NetworkProxyDetail Unconfigured { get; } = new();

    /// <summary>
    /// The enabled state of the network proxy.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The proxy server of the network proxy.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// The port of the network proxy.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// The username of the network proxy.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The password of the network proxy.
    /// </summary>
    public string? Password { get; set; }
}
