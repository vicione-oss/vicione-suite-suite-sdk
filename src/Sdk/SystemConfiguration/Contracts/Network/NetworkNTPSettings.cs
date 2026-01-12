using System.Text.Json.Serialization;
using Sdk.SystemConfiguration.Contracts.Extensions;

namespace Sdk.SystemConfiguration.Contracts.Network;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VO2001 // Members are not grouped by type, then sorted in predefined order

/// <summary>
/// Represents the <see cref="NetworkNTPSettings"/>.
/// </summary>
public sealed record NetworkNTPSettings
{
    /// <summary>
    /// Initializes new <see cref="NetworkNTPSettings"/>.
    /// </summary>
    [JsonConstructor]
    public NetworkNTPSettings(List<string>? ntpServers = null)
        => NTPServers = ntpServers ?? [];

    /// <summary>
    /// Represents the empty NTP settings.
    /// </summary>
    [JsonIgnore]
    public static NetworkNTPSettings Empty { get; } = new NetworkNTPSettings();

    /// <summary>
    /// Determines whether NTP servers are enabled.
    /// </summary>
    public bool NTPServersEnabled { get; set; }

    /// <summary>
    /// The list of NTP servers (IPv4, IPv6 addresses and DNS host names).
    /// </summary>
    public List<string> NTPServers { get; init; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(NTPServersEnabled);
        hashCode.Add(NTPServers.GetSequenceHashCode());

        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(NetworkNTPSettings? other) =>
        other is not null &&
        NTPServersEnabled.Equals(other.NTPServersEnabled) &&
        NTPServers.SequenceEqual(other.NTPServers);
}

#pragma warning restore VO2001 // Members are not grouped by type, then sorted in predefined order
#pragma warning restore IDE0079 // Remove unnecessary suppression
