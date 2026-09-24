using Sdk.Messaging;
using Sdk.SystemConfiguration.Contracts;

namespace Sdk.SystemConfiguration;

/// <summary>
/// Represents the result of a service control management operation.
/// </summary>
[ExcludeFromCodeCoverage]
public record ControlServiceManagementResult(string ServiceName, ServiceState State, ErrorInfo? Error = null)
{
    /// <summary>
    /// Gets whether the operation was successful.
    /// </summary>
    public bool Success => Error is null;
}
