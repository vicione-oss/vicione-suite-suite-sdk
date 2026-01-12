using Sdk.Messaging;

namespace Sdk.SystemConfiguration;

/// <summary>
/// Represents a command to control a system service.
/// </summary>
public record ControlService(string ServiceName, ServiceCommand Command) : IInstanceDependentCommand
{
    /// <summary>
    /// Gets the unique identifier for this command instance, used for correlation.
    /// </summary>
    public Guid CorrelationId { get; } = Guid.NewGuid();
}

/// <summary>
/// Defines the set of commands that can be executed on a system service.
/// </summary>
public enum ServiceCommand
{
    /// <summary>
    /// Starts the service.
    /// </summary>
    Start,

    /// <summary>
    /// Stops the service.
    /// </summary>
    Stop,

    /// <summary>
    /// Restarts the service.
    /// </summary>
    Restart,

    /// <summary>
    /// Enables the service.
    /// </summary>
    Enable,

    /// <summary>
    /// Disables the service.
    /// </summary>
    Disable
}
