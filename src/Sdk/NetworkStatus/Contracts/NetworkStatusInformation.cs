using System.Text.Json.Serialization;

namespace Sdk.NetworkStatus.Contracts;

/// <summary>
/// Represents the <see cref="NetworkStatusInformation"/>.
/// </summary>
public sealed record NetworkStatusInformation
{
    /// <summary>
    /// Represents the empty <see cref="NetworkStatusInformation"/>.
    /// </summary>
    [JsonIgnore]
    public static NetworkStatusInformation Empty { get; } = new NetworkStatusInformation()
    {
        IsDefaultRouteConfigured = false,
        IsDefaultGatewayAvailable = false,
        IsInternetAvailable = false,
        IsDNSFunctional = false,
        InterfaceState = "Unknown"
    };

    /// <summary>
    /// Determines whether a default route is configured.
    /// </summary>
    public required bool IsDefaultRouteConfigured { get; init; }

    /// <summary>
    /// Determines whether a default gateway is available.
    /// </summary>
    public required bool IsDefaultGatewayAvailable { get; init; }

    /// <summary>
    /// Determines whether an internet connection is available.
    /// </summary>
    public required bool IsInternetAvailable { get; init; }

    /// <summary>
    /// Determines whether domain name resolution is functional.
    /// </summary>
    public required bool IsDNSFunctional { get; init; }

    /// <summary>
    /// The connection state of the interface.
    /// </summary>
    public required string InterfaceState { get; init; }
}
