namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents DHCP lease information for a network interface.
/// </summary>
/// <remarks>
/// The presence of this object on a <see cref="NetworkInterface"/> indicates that DHCP is active.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record DhcpLeaseInfo
{
    /// <summary>
    /// Gets or initializes the date and time the lease was obtained, or <see langword="null"/> if not available.
    /// </summary>
    public DateTimeOffset? LeaseObtained { get; init; }

    /// <summary>
    /// Gets or initializes the date and time the lease will expire, or <see langword="null"/> if not available.
    /// </summary>
    public DateTimeOffset? LeaseExpires { get; init; }
}

