using Sdk.Messaging;

namespace Sdk.NetworkStatus.Requests;

/// <summary>
/// Represents a request to retrieve network status information for a specific network interface.
/// </summary>
public record GetNetworkStatusInformation(string NetworkInterfaceName) : IRequest<GetNetworkStatusInformationResponse>;
