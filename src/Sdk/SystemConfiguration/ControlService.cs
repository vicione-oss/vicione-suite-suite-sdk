using Sdk.Messaging;

namespace Sdk.SystemConfiguration;

public record ControlService(string ServiceName, ServiceCommand Command) : IInstanceDependentCommand
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
}

public enum ServiceCommand
{
    Start,
    Stop,
    Restart,
    Enable,
    Disable,
}
