using System.Net;
using System.Text.Json.Serialization;
using Sdk.SystemConfiguration.Contracts.Extensions;

namespace Sdk.SystemConfiguration.Contracts.Network;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VO2001 // Members are not grouped by type, then sorted in predefined order

/// <summary>
/// Represents the <see cref="IPv4Settings"/>.
/// </summary>
public sealed record IPv4Settings
{
    /// <summary>
    /// Initializes new <see cref="IPv4Settings"/>.
    /// </summary>
    [JsonConstructor]
    public IPv4Settings(List<IPv4Detail>? ipv4Details = null)
        => IPv4Details = ipv4Details ?? [];

    /// <summary>
    /// Represents the empty IPv4 settings.
    /// </summary>
    [JsonIgnore]
    public static IPv4Settings Empty { get; } = new IPv4Settings();

    /// <summary>
    /// The IP address of the default gateway.
    /// </summary>
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress? Gateway { get; set; }

    /// <summary>
    /// Determines if DHCP should be enabled.
    /// </summary>
    public bool DHCPEnabled { get; set; }

    /// <summary>
    /// The list of IPv4 details.
    /// </summary>
    public List<IPv4Detail> IPv4Details { get; init; }

    /// <summary>
    /// Determines if VLAN should be enabled.
    /// </summary>
    public bool VLANEnabled { get; set; }

    /// <summary>
    /// The VLAN ID.
    /// </summary>
    /// <remarks>
    /// <b>0:</b> indicates that the frame does not carry a VLAN ID<br/>
    /// <b>1-4094:</b> usable VLAN IDs<br/>
    /// <b>4095:</b> reserved for implementation use and can not be configured
    /// </remarks>
    public int VLANID { get; set; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Gateway);
        hashCode.Add(DHCPEnabled);
        hashCode.Add(IPv4Details.GetSequenceHashCode());
        hashCode.Add(VLANEnabled);
        hashCode.Add(VLANID);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(IPv4Settings? other) =>
        other is not null &&
        EqualityComparer<IPAddress>.Default.Equals(Gateway, other.Gateway) &&
        DHCPEnabled == other.DHCPEnabled &&
        IPv4Details.SequenceEqual(other.IPv4Details) &&
        VLANEnabled == other.VLANEnabled &&
        VLANID == other.VLANID;
}

#pragma warning restore VO2001 // Members are not grouped by type, then sorted in predefined order
#pragma warning restore IDE0079 // Remove unnecessary suppression
