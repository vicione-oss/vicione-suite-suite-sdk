using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents the DHCP lease information.
/// </summary>
public sealed record DHCPLease
{
    /// <summary>
    /// Represents the empty DHCP lease object.
    /// </summary>
    [JsonIgnore]
    public static DHCPLease Empty { get; } = new();

    /// <summary>
    /// The date and time the lease was obtained.
    /// </summary>
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? LeaseObtained { get; init; }

    /// <summary>
    /// The date and time the lease will expire.
    /// </summary>
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? LeaseExpires { get; init; }

    /// <summary>
    /// The IP address of the default gateway.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress? Gateway { get; init; }

    /// <summary>
    /// The DNS settings.
    /// </summary>
    public NetworkDNSSettings? NetworkDNSSettings { get; init; }

    /// <summary>
    /// The IPv4 detail.
    /// </summary>
    public IPv4Detail? IPv4Detail { get; init; }
}
