using Sdk.Messaging;

namespace Sdk.SystemConfiguration;

/// <summary>
/// Use this interface to start/stop services on linux systems. 
/// If used within <see cref="MassTransit.IConsumer"/> ensure it has the <see cref="ReadOnlyConsumerAttribute"/>
/// </summary>
public interface IControlServiceManagement
{
    /// <summary>
    /// Start/stop service using local HostManagement on Linux systems
    /// </summary>    
    Task<ControlServiceManagementResult> ControlService(ServiceCommand command, string serviceName, CancellationToken cancellationToken = default);
}
