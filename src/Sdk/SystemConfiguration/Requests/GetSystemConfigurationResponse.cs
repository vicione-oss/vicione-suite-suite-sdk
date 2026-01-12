using Sdk.Messaging;

namespace Sdk.SystemConfiguration.Requests;

public sealed record GetSystemConfigurationResponse : IResponse
{
    public Contracts.SystemConfiguration? Configuration { get; init; }
    public ErrorInfo? RequestError { get; init; }
}
