using Sdk.SystemConfiguration.Contracts.Extensions;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VO2001 // Members are not grouped by type, then sorted in predefined order

/// <summary>
/// Represents the <see cref="NetworkInterfacesSettings"/>.
/// </summary>
public sealed record NetworkInterfacesSettings
{
    /// <summary>
    /// Initializes new <see cref="NetworkInterfacesSettings"/>.
    /// </summary>
    [JsonConstructor]
    public NetworkInterfacesSettings(List<NetworkInterfaceDetail>? networkInterfaces = null)
        => NetworkInterfaces = networkInterfaces ?? [];

    /// <summary>
    /// The list of network interfaces.
    /// </summary>
    public List<NetworkInterfaceDetail> NetworkInterfaces { get; init; }

    /// <inheritdoc/>
    public override int GetHashCode() => NetworkInterfaces.GetSequenceHashCode();

    /// <inheritdoc/>
    public bool Equals(NetworkInterfacesSettings? other) =>
        other is not null &&
        NetworkInterfaces.SequenceEqual(other.NetworkInterfaces);
}

#pragma warning restore VO2001 // Members are not grouped by type, then sorted in predefined order
#pragma warning restore IDE0079 // Remove unnecessary suppression
