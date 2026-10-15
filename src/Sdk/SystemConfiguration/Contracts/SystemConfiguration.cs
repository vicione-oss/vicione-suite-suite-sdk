using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the read-only system configuration of the target system.
/// </summary>
/// <remarks>
/// This configuration is provided by the HostManagement system and mapped to this simplified, module-friendly representation.
/// </remarks>
public sealed record SystemConfiguration
{
    /// <summary>
    /// Gets or initializes the schema version of this SDK contract, which is independent of the HostManagement configuration version.
    /// </summary>
    [JsonRequired]
    public int Version { get; init; } = 1;

    /// <summary>
    /// Gets or initializes the list of network interfaces.
    /// </summary>
    public IReadOnlyList<NetworkInterface> NetworkInterfaces { get; init => field = value ?? []; } = [];

    /// <summary>
    /// Gets or initializes the DNS settings.
    /// </summary>
    public DnsSettings Dns { get; init => field = value ?? new(); } = new();

    /// <summary>
    /// Gets or initializes the proxy settings.
    /// </summary>
    public ProxySettings Proxy { get; init => field = value ?? new(); } = new();

    /// <summary>
    /// Gets or initializes the NTP settings.
    /// </summary>
    public NtpSettings Ntp { get; init => field = value ?? new(); } = new();

    /// <summary>
    /// Gets or initializes the list of managed services.
    /// </summary>
    public IReadOnlyList<ServiceInfo> Services { get; init => field = value ?? []; } = [];
}
