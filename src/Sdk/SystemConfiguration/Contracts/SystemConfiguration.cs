using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the read-only system configuration of the target system.
/// </summary>
/// <remarks>
/// This configuration is provided by the HostManagement system and mapped to this simplified, module-friendly representation.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record SystemConfiguration
{
    /// <summary>
    /// Gets or initializes the version of the system configuration schema.
    /// </summary>
    [JsonRequired]
    public int Version { get; init; } = 1;

    /// <summary>
    /// Gets or initializes the list of network interfaces.
    /// </summary>
    public IReadOnlyList<NetworkInterface> NetworkInterfaces { get; init; } = [];

    /// <summary>
    /// Gets or initializes the DNS settings.
    /// </summary>
    public DnsSettings Dns { get; init; } = new();

    /// <summary>
    /// Gets or initializes the proxy settings.
    /// </summary>
    public ProxySettings Proxy { get; init; } = new();

    /// <summary>
    /// Gets or initializes the NTP settings.
    /// </summary>
    public NtpSettings Ntp { get; init; } = new();

    /// <summary>
    /// Gets or initializes the list of managed services.
    /// </summary>
    public IReadOnlyList<ServiceInfo> Services { get; init; } = [];
}
