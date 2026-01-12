using Sdk.Instance;

namespace Sdk.Testing.Backend;

/// <summary>
/// A test implementation of the <see cref="IInstanceInformation"/> interface.
/// </summary>
public class TestInstanceInformation : IInstanceInformation
{
    /// <inheritdoc/>
    public Guid Id { get; init; }

    /// <inheritdoc/>
    public InstanceType Type { get; init; }

    /// <inheritdoc/>
    public string? Name { get; init; }

    /// <inheritdoc/>
    public string FormattedName { get; } = "{ViciOne} Suite";

    /// <inheritdoc/>
    public string? Description { get; init; }

    /// <inheritdoc/>
    public string SerialNumber => Id.ToString("N");

    /// <inheritdoc/>
    public IReadOnlyCollection<string> InstalledModules { get; init; } = [];

    /// <inheritdoc/>
    public DateTimeOffset? FirstTimeRegistered { get; } = DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public DateTimeOffset? LastRegistered { get; } = DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public string Version { get; init; } = "undefined";

    /// <inheritdoc/>
    public string? BranchName { get; init; }

    /// <inheritdoc/>
    public string SdkVersion { get; init; } = "undefined";

    /// <inheritdoc/>
    public bool InRecoveryMode { get; set; }
}
