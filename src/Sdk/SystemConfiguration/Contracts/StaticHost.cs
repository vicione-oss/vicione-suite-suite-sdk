using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents a static host entry mapping a hostname to an IP address.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record StaticHost
{
    /// <summary>
    /// Gets or initializes the IP address of the static host.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress IpAddress { get; init; }

    /// <summary>
    /// Gets or initializes the hostname of the static host.
    /// </summary>
    public required string Hostname { get; init; }
}

