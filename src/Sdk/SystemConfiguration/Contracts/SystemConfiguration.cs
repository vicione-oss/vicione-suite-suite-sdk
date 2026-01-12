using System.Text.Json.Serialization;
using Sdk.SystemConfiguration.Contracts.Extensions;
using Sdk.SystemConfiguration.Contracts.Network;
using Sdk.SystemConfiguration.Contracts.Service;

namespace Sdk.SystemConfiguration.Contracts;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable VO2001 // Members are not grouped by type, then sorted in predefined order

/// <summary>
/// Represents the <see cref="SystemConfiguration"/> of the target system.
/// </summary>
[method: JsonConstructor]
public class SystemConfiguration(List<ServiceDetail>? services = null)
{
    /// <summary>
    /// The version of the system configuration.
    /// </summary>
    [JsonRequired]
    public int Version { get; init; } = 1;

    /// <summary>
    /// The <see cref="NetworkInterfacesSettings"/>.
    /// </summary>
    public NetworkInterfacesSettings NetworkInterfacesSettings { get; set; } = new();

    /// <summary>
    /// The <see cref="NetworkDNSSettings"/>.
    /// </summary>
    public NetworkDNSSettings NetworkDNSSettings { get; set; } = new();

    /// <summary>
    /// The <see cref="NetworkProxySettings"/>.
    /// </summary>
    public NetworkProxySettings NetworkProxySettings { get; set; } = new();

    /// <summary>
    /// The <see cref="NetworkNTPSettings"/>.
    /// </summary>
    public NetworkNTPSettings NetworkNTPSettings { get; set; } = new();

    /// <summary>
    /// The list of managed services.
    /// </summary>
    public List<ServiceDetail> Services { get; init; } = services ?? [];

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(
            NetworkInterfacesSettings,
            NetworkDNSSettings,
            NetworkProxySettings,
            NetworkNTPSettings,
            Services.OrderBy(x => x.Name).GetSequenceHashCode());

    /// <inheritdoc/>
    public bool Equals(SystemConfiguration? other) =>
        other is not null &&
        NetworkInterfacesSettings.Equals(other.NetworkInterfacesSettings) &&
        NetworkDNSSettings.Equals(other.NetworkDNSSettings) &&
        NetworkProxySettings.Equals(other.NetworkProxySettings) &&
        NetworkNTPSettings.Equals(other.NetworkNTPSettings) &&
        Services.OrderBy(x => x.Name).SequenceEqual(other.Services.OrderBy(x => x.Name));
}

#pragma warning restore VO2001 // Members are not grouped by type, then sorted in predefined order
#pragma warning restore IDE0079 // Remove unnecessary suppression
