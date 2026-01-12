using Sdk.Messaging;

namespace Sdk.NetworkStatus.Requests;

public record GetNetworkStatusInformation(string NetworkInterfaceName) : IRequest<GetNetworkStatusInformationResponse>;
