namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents a VLAN configuration for a network interface.
/// </summary>
/// <remarks>
/// The presence of this object on a <see cref="NetworkInterface"/> indicates that VLAN is active.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record VlanInfo
{
    /// <summary>
    /// Gets or initializes the VLAN ID.
    /// </summary>
    /// <remarks>
    /// <b>1–4094:</b> usable VLAN IDs.
    /// </remarks>
    public required int Id { get; init; }
}

