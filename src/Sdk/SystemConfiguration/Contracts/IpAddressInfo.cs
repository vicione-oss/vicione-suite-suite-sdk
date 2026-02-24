using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents an IP address and netmask pair.
/// </summary>
/// <remarks>
/// Use <see cref="AddressFamily"/> to determine whether this entry represents an IPv4 or IPv6 address.
/// Currently only <see cref="System.Net.Sockets.AddressFamily.InterNetwork"/> (IPv4) is used;
/// IPv6 support is planned for a future release.
/// </remarks>
public sealed record IpAddressInfo
{
    /// <summary>
    /// Indicates whether this entry is an IPv4 (<see cref="System.Net.Sockets.AddressFamily.InterNetwork"/>)
    /// or IPv6 (<see cref="System.Net.Sockets.AddressFamily.InterNetworkV6"/>) address.
    /// </summary>
    public required AddressFamily AddressFamily { get; init; }

    /// <summary>
    /// The IP address.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress IpAddress { get; init; }

    /// <summary>
    /// The netmask.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress Netmask { get; init; }
}

