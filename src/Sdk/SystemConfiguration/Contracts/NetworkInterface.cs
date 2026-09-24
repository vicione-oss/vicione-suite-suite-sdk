using System.Net;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents a network interface and its current effective configuration.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="IPv4Address"/>, <see cref="IPv4Netmask"/>, and <see cref="IPv4Gateway"/> properties
/// always reflect the currently active IPv4 values — whether they originate from a static configuration
/// or were assigned via DHCP.
/// </para>
/// <para>
/// When DHCP is active and a lease has been obtained, <see cref="DhcpLease"/> contains the lease details.
/// When DHCP is not active, <see cref="DhcpLease"/> is <see langword="null"/>.
/// </para>
/// <para>
/// When VLAN is active, <see cref="Vlan"/> contains the VLAN configuration.
/// When VLAN is not active, <see cref="Vlan"/> is <see langword="null"/>.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record NetworkInterface
{
    /// <summary>
    /// Gets or initializes the name of the network interface (e.g. "lan1").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or initializes the physical (MAC) address of the network interface.
    /// </summary>
    public string PhysicalAddress { get; init; } = string.Empty;

    /// <summary>
    /// Gets or initializes whether the network interface is enabled.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Gets or initializes the currently effective primary IPv4 address of the network interface.
    /// </summary>
    /// <remarks>
    /// This is the active IPv4 address regardless of whether it was statically configured or assigned via DHCP.
    /// </remarks>
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress? IPv4Address { get; init; }

    /// <summary>
    /// Gets or initializes the currently effective IPv4 netmask of the network interface.
    /// </summary>
    /// <remarks>
    /// This is the active IPv4 netmask regardless of whether it was statically configured or assigned via DHCP.
    /// </remarks>
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress? IPv4Netmask { get; init; }

    /// <summary>
    /// Gets or initializes the currently effective IPv4 default gateway of the network interface.
    /// </summary>
    /// <remarks>
    /// This is the active IPv4 gateway regardless of whether it was statically configured or assigned via DHCP.
    /// </remarks>
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress? IPv4Gateway { get; init; }

    /// <summary>
    /// Gets or initializes the DHCP lease information if DHCP is enabled and a lease has been obtained;
    /// otherwise <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// A non-null value indicates that DHCP is active on this interface.
    /// </remarks>
    public DhcpLeaseInfo? DhcpLease { get; init; }

    /// <summary>
    /// Gets or initializes the VLAN configuration if VLAN is enabled on this interface;
    /// otherwise <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// A non-null value indicates that VLAN is active on this interface.
    /// </remarks>
    public VlanInfo? Vlan { get; init; }

    /// <summary>
    /// Gets or initializes the additional IP addresses configured on this interface beyond the primary <see cref="IPv4Address"/>.
    /// </summary>
    public IReadOnlyList<IpAddressInfo> AdditionalAddresses { get; init; } = [];
}

