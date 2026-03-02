namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents the proxy settings of the system.
/// </summary>
/// <remarks>
/// <para>
/// Each proxy type (<see cref="Http"/>, <see cref="Https"/>, etc.) is <see langword="null"/>
/// when the corresponding proxy is not enabled in the system configuration.
/// A non-null value indicates that the proxy is active and configured.
/// </para>
/// <para>
/// <see cref="DoNotProxyList"/> is only populated when the do-not-proxy feature is enabled
/// in the system configuration. An empty collection indicates the feature is either disabled
/// or has no entries configured.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed record ProxySettings
{
    /// <summary>
    /// The HTTP proxy configuration, or <see langword="null"/> if not enabled.
    /// </summary>
    public ProxyInfo? Http { get; init; }

    /// <summary>
    /// The HTTPS proxy configuration, or <see langword="null"/> if not enabled.
    /// </summary>
    public ProxyInfo? Https { get; init; }

    /// <summary>
    /// The FTP proxy configuration, or <see langword="null"/> if not enabled.
    /// </summary>
    public ProxyInfo? Ftp { get; init; }

    /// <summary>
    /// The SFTP proxy configuration, or <see langword="null"/> if not enabled.
    /// </summary>
    public ProxyInfo? Sftp { get; init; }

    /// <summary>
    /// The SOCKS proxy configuration, or <see langword="null"/> if not enabled.
    /// </summary>
    public ProxyInfo? Socks { get; init; }

    /// <summary>
    /// The list of IPv4/IPv6 addresses or domain names that should bypass the proxy.
    /// </summary>
    /// <remarks>
    /// Only populated when the do-not-proxy list is enabled in the system configuration.
    /// <para>
    /// If a <b>port</b> needs to be specified the following format is valid:
    /// IPv4:PORT or [IPv6]:PORT.
    /// </para>
    /// <para>
    /// If a <b>link-local IPv6</b> address is used, a <b>zone identifier</b> is appended,
    /// e.g. <c>fe80::1%eth0</c>. When used in a URI, <c>%</c> must be percent-encoded as
    /// <c>%25</c> to satisfy RFC 6874.
    /// </para>
    /// </remarks>
    public IReadOnlyList<string> DoNotProxyList { get; init; } = [];
}

