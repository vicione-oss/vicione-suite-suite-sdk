using System.Text.Json.Serialization;

namespace Sdk.NetworkStatus.Contracts;

/// <summary>
/// The connectivity state of one network interface.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record NetworkStatusInformation
{
    /// <summary>
    /// Gets the value that reports every check as failed and the interface state as <c>Unknown</c>.
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
    /// Gets or initializes whether a default route is configured.
    /// </summary>
    public required bool IsDefaultRouteConfigured { get; init; }

    /// <summary>
    /// Gets or initializes whether a default gateway is available.
    /// </summary>
    public required bool IsDefaultGatewayAvailable { get; init; }

    /// <summary>
    /// Gets or initializes whether an internet connection is available.
    /// </summary>
    public required bool IsInternetAvailable { get; init; }

    /// <summary>
    /// Gets or initializes whether domain name resolution is functional.
    /// </summary>
    public required bool IsDNSFunctional { get; init; }

    /// <summary>
    /// Gets or initializes the connection state of the interface.
    /// </summary>
    public required string InterfaceState { get; init; }
}
