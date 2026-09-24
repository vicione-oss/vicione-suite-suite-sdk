namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents information about a managed system service.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record ServiceInfo
{
    /// <summary>
    /// Gets or initializes the service name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or initializes the current state of the service.
    /// </summary>
    public required ServiceState State { get; init; }
}

