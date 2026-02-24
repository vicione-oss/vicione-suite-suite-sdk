using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents a static host entry mapping a hostname to an IP address.
/// </summary>
public sealed record StaticHost
{
    /// <summary>
    /// The IP address of the static host.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress IpAddress { get; init; }

    /// <summary>
    /// The hostname of the static host.
    /// </summary>
    public required string Hostname { get; init; }
}

