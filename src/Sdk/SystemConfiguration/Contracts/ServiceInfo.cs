namespace Sdk.SystemConfiguration.Contracts;

/// <summary>
/// Represents information about a managed system service.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record ServiceInfo
{
    /// <summary>
    /// The service name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The current state of the service.
    /// </summary>
    public required ServiceState State { get; init; }
}

