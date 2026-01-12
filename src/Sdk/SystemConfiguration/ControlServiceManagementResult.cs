using Sdk.Messaging;
using Sdk.SystemConfiguration.Contracts.Service;

namespace Sdk.SystemConfiguration;

public record ControlServiceManagementResult(string ServiceName, ServiceState State, ErrorInfo? Error = null)
{
    public bool Success => Error is null;
}
