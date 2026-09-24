using Sdk.Messaging;
using Sdk.SystemConfiguration.Commands;

namespace Sdk.SystemConfiguration;

/// <summary>
/// Defines a contract for managing system services on a host.
/// </summary>
/// <remarks>
/// If injected into a <see cref="MassTransit.IConsumer"/>, ensure the consumer is annotated with <see cref="ReadOnlyConsumerAttribute"/>.
/// </remarks>
public interface IControlServiceManagement
{
    /// <summary>
    /// Gets whether services can be controlled on the current system.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Sends <paramref name="command"/> to the service; a failure is reported in the result, not thrown.
    /// </summary>
    Task<ControlServiceManagementResult> TryControlService(ServiceCommand command, string serviceName,
        CancellationToken cancellationToken = default);
}
