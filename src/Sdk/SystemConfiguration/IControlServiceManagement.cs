using Sdk.Messaging;

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
    /// Asynchronously sends a command to control a system service.
    /// </summary>
    Task<ControlServiceManagementResult> ControlService(ServiceCommand command, string serviceName,
        CancellationToken cancellationToken = default);
}
