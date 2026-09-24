using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the DNS settings of the system.
/// </summary>
/// <remarks>
/// <para>
/// Collections such as <see cref="NameServers"/>, <see cref="SearchDomains"/>, and <see cref="StaticHosts"/>
/// are only populated when the corresponding feature is enabled in the underlying system configuration.
/// An empty collection indicates the feature is either disabled or has no entries configured.
/// </para>
/// <para>
/// <see cref="DnsSuffix"/> is only populated when the DNS suffix feature is enabled.
/// An empty string indicates the feature is disabled or no suffix is configured.
/// </para>
/// <para>
/// <see cref="MulticastDnsEnabled"/> reflects whether multicast DNS (mDNS) is enabled on the system.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record DnsSettings
{
    /// <summary>
    /// Gets or initializes the hostname of the system.
    /// </summary>
    public string Hostname { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes whether multicast DNS (mDNS) is enabled on the system.
    /// </summary>
    public bool MulticastDnsEnabled { get; init; }

    /// <summary>
    /// Gets or initializes the list of DNS name servers (IPv4 and IPv6 addresses).
    /// </summary>
    /// <remarks>
    /// Only populated when name servers are enabled in the system configuration.
    /// </remarks>
    [JsonConverter(typeof(IPAddressListConverter))]
    public IReadOnlyList<IPAddress> NameServers { get; init; } = [];

    /// <summary>
    /// Gets or initializes the primary DNS suffix.
    /// </summary>
    /// <remarks>
    /// Only populated when the DNS suffix feature is enabled in the system configuration.
    /// </remarks>
    public string DnsSuffix { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes the list of DNS search domains, excluding the primary DNS suffix.
    /// </summary>
    /// <remarks>
    /// Only populated when search domains are enabled in the system configuration.
    /// </remarks>
    public IReadOnlyList<string> SearchDomains { get; init; } = [];

    /// <summary>
    /// Gets or initializes the list of static host entries.
    /// </summary>
    /// <remarks>
    /// Only populated when static hosts are enabled in the system configuration.
    /// </remarks>
    public IReadOnlyList<StaticHost> StaticHosts { get; init; } = [];
}

