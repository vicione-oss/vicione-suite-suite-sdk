namespace Sdk.Instance;
/// <summary>
/// Provides detailed information about a specific instance within the system,
/// including identity, versioning, state, and installed modules.
/// </summary>
public interface IInstanceInformation
{
    /// <summary>
    /// Gets the instance's unique ID.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the instance's role: standalone, master or slave.
    /// </summary>
    InstanceType Type { get; }

    /// <summary>
    /// Gets the instance's display name; <see langword="null"/> if none is set.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the name formatted for display or logging.
    /// </summary>
    string FormattedName { get; }

    /// <summary>
    /// Gets the instance's description, e.g. its purpose; <see langword="null"/> if none is set.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the instance's unique serial number.
    /// </summary>
    string SerialNumber { get; }

    /// <summary>
    /// Gets the type of the platform the instance runs on.
    /// </summary>
    string SystemType { get; }

    /// <summary>
    /// Gets the IDs of the modules installed on the instance.
    /// </summary>
    IReadOnlyCollection<string> InstalledModules { get; }

    /// <summary>
    /// Gets when the instance was first registered; <see langword="null"/> if unknown.
    /// </summary>
    DateTimeOffset? FirstTimeRegistered { get; }

    /// <summary>
    /// Gets when the instance was last registered or updated; <see langword="null"/> if unknown.
    /// </summary>
    DateTimeOffset? LastRegistered { get; }

    /// <summary>
    /// Gets the version of the instance's software.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the source control branch the instance was built from; <see langword="null"/> if unknown.
    /// </summary>
    string? BranchName { get; }

    /// <summary>
    /// Gets the version of the SDK the instance was built with.
    /// </summary>
    string SdkVersion { get; }

    /// <summary>
    /// Gets whether the instance runs in recovery mode, without loading its modules.
    /// </summary>
    bool InRecoveryMode { get; }
}

