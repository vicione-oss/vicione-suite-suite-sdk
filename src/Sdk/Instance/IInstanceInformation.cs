namespace Sdk.Instance;
/// <summary>
/// Provides detailed information about a specific instance within the system,
/// including identity, versioning, state, and installed modules.
/// </summary>
public interface IInstanceInformation
{
    /// <summary>
    /// Unique identifier of the instance
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Type of the instance, such as backend or client
    /// </summary>
    InstanceType Type { get; }

    /// <summary>
    /// Optional display name of the instance, if provided
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Pre-formatted name for the instance, suitable for UI display or logging
    /// </summary>
    string FormattedName { get; }

    /// <summary>
    /// Optional description of the instance, such as its purpose or environment
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Unique serial number assigned to the instance
    /// </summary>
    string SerialNumber { get; }

    /// <summary>
    /// Read-only collection of module identifiers that are installed on this instance
    /// </summary>
    IReadOnlyCollection<string> InstalledModules { get; }

    /// <summary>
    /// Timestamp when this instance was first registered in the system, if available
    /// </summary>
    DateTimeOffset? FirstTimeRegistered { get; }

    /// <summary>
    /// Timestamp when this instance was last registered or updated in the system, if available
    /// </summary>
    DateTimeOffset? LastRegistered { get; }

    /// <summary>
    /// Version of the instance's software
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Source control branch name the instance is running from, if available
    /// </summary>
    string? BranchName { get; }

    /// <summary>
    /// Version of the SDK that this instance was built with
    /// </summary>
    string SdkVersion { get; }

    /// <summary>
    /// Value indicating whether the instance is running in recovery mode without loading its modules
    /// </summary>
    bool InRecoveryMode { get; }
}

