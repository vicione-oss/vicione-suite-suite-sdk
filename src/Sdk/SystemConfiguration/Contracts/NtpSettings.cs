namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the NTP settings of the system.
/// </summary>
/// <remarks>
/// <see cref="Servers"/> is only populated when NTP servers are enabled in the system configuration.
/// An empty collection indicates NTP is either disabled or has no servers configured.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record NtpSettings
{
    /// <summary>
    /// Gets or initializes the list of configured NTP servers (IPv4/IPv6 addresses or DNS hostnames).
    /// </summary>
    /// <remarks>
    /// Only populated when NTP servers are enabled in the system configuration.
    /// </remarks>
    public IReadOnlyList<string> Servers { get; init; } = [];

    /// <summary>
    /// Gets or initializes the list of fallback NTP servers (IPv4/IPv6 addresses or DNS hostnames).
    /// </summary>
    public IReadOnlyList<string> FallbackServers { get; init; } = [];
}

