using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents a <see cref="StaticHostDetail"/>.
/// </summary>
public sealed record StaticHostDetail
{
    /// <summary>
    /// The IP address.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public required IPAddress IPAddress { get; set; }

    /// <summary>
    /// The hostname.
    /// </summary>
    public required string Hostname { get; set; }
}
