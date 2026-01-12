using Sdk.Messaging;
using Sdk.NetworkStatus.Contracts;

namespace Sdk.NetworkStatus.Requests;

public sealed record GetNetworkStatusInformationResponse : IResponse
{
    public NetworkStatusInformation? NetworkStatusInformation { get; init; }
    public ErrorInfo? RequestError { get; init; }
}
