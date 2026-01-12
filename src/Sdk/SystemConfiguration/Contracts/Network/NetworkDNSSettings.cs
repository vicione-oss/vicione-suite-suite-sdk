using System.Net;
using System.Text.Json.Serialization;
using Sdk.SystemConfiguration.Contracts.Extensions;

namespace Sdk.SystemConfiguration.Contracts.Network;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VO2001 // Members are not grouped by type, then sorted in predefined order

/// <summary>
/// Represents the <see cref="NetworkDNSSettings"/>.
/// </summary>
public sealed record NetworkDNSSettings
{
    /// <summary>
    /// Initializes new <see cref="NetworkDNSSettings"/>.
    /// </summary>
    [JsonConstructor]
    public NetworkDNSSettings(
        List<StaticHostDetail>? staticHosts = null,
        List<IPAddress>? nameServers = null,
        List<string>? searchDomains = null)
    {
        StaticHosts = staticHosts ?? [];
        NameServers = nameServers ?? [];
        SearchDomains = searchDomains ?? [];
    }

    /// <summary>
    /// Represents the empty DNS settings.
    /// </summary>
    [JsonIgnore]
    public static NetworkDNSSettings Empty { get; } = new NetworkDNSSettings();

    /// <summary>
    /// The hostname.
    /// </summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether multicast DNS (mDNS) is enabled.
    /// </summary>
    public bool MulticastDNSEnabled { get; set; }

    /// <summary>
    /// Determines whether static hosts are enabled.
    /// </summary>
    public bool StaticHostsEnabled { get; set; }

    /// <summary>
    /// The list of static host details.
    /// </summary>
    public List<StaticHostDetail> StaticHosts { get; init; }

    /// <summary>
    /// Determines whether the DNS name servers are enabled.
    /// </summary>
    public bool NameServersEnabled { get; set; }

    /// <summary>
    /// The list of DNS name servers (IPv4 and IPv6 addresses).
    /// </summary>
    [JsonConverter(typeof(IPAddressListConverter))]
    public List<IPAddress> NameServers { get; init; }

    /// <summary>
    /// Determines whether the primary DNS suffix is enabled.
    /// </summary>
    public bool DNSSuffixEnabled { get; set; }

    /// <summary>
    /// The primary DNS suffix.
    /// </summary>
    public string DNSSuffix { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether search domains are enabled.
    /// </summary>
    public bool SearchDomainsEnabled { get; set; }

    /// <summary>
    /// The list of DNS search domains.
    /// </summary>
    /// <remarks>
    /// DNS suffixes excluding the primary DNS suffix (<see cref="DNSSuffix"/>).
    /// </remarks>
    public List<string> SearchDomains { get; init; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Hostname);
        hashCode.Add(MulticastDNSEnabled);
        hashCode.Add(StaticHostsEnabled);
        hashCode.Add(StaticHosts.GetSequenceHashCode());
        hashCode.Add(NameServersEnabled);
        hashCode.Add(NameServers.GetSequenceHashCode());
        hashCode.Add(DNSSuffixEnabled);
        hashCode.Add(DNSSuffix);
        hashCode.Add(SearchDomainsEnabled);
        hashCode.Add(SearchDomains.GetSequenceHashCode());

        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(NetworkDNSSettings? other) =>
        other is not null &&
#pragma warning disable CA1307 // Specify StringComparison for clarity
#pragma warning disable CA1309 // Use ordinal string comparison
        Hostname.Equals(other.Hostname) &&
#pragma warning restore CA1309 // Use ordinal string comparison
#pragma warning restore CA1307 // Specify StringComparison for clarity
        MulticastDNSEnabled.Equals(other.MulticastDNSEnabled) &&
        StaticHostsEnabled.Equals(other.StaticHostsEnabled) &&
        StaticHosts.SequenceEqual(other.StaticHosts) &&
        NameServersEnabled.Equals(other.NameServersEnabled) &&
        NameServers.SequenceEqual(other.NameServers) &&
        DNSSuffixEnabled.Equals(other.DNSSuffixEnabled) &&
#pragma warning disable CA1307 // Specify StringComparison for clarity
#pragma warning disable CA1309 // Use ordinal string comparison
        DNSSuffix.Equals(other.DNSSuffix) &&
#pragma warning restore CA1309 // Use ordinal string comparison
#pragma warning restore CA1307 // Specify StringComparison for clarity
        SearchDomainsEnabled.Equals(other.SearchDomainsEnabled) &&
        SearchDomains.SequenceEqual(other.SearchDomains);
}

#pragma warning restore VO2001 // Members are not grouped by type, then sorted in predefined order
#pragma warning restore IDE0079 // Remove unnecessary suppression

