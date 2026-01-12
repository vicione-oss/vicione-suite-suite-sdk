namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents the <see cref="NetworkInterfaceCommonInformation"/>.
/// </summary>
public sealed record NetworkInterfaceCommonInformation
{
    /// <summary>
    /// The enabled state of the network interface.
    /// </summary>
    public required bool Enabled { get; set; }

    /// <summary>
    /// The name of the network interface.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The physical address of the network interface.
    /// </summary>
    public string PhysicalAddress { get; set; } = string.Empty;
}
