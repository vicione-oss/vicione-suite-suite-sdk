using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents an <see cref="IPv4Detail"/>.
/// </summary>
public sealed record IPv4Detail
{
    /// <summary>
    /// The IP address.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress IPAddress { get; set; }

    /// <summary>
    /// The netmask.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress Netmask { get; set; }
}
