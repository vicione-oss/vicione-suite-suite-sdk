using Sdk.Messaging;
using Sdk.NetworkStatus.Contracts;

namespace Sdk.NetworkStatus.Requests;

/// <summary>
/// Represents the response to a request for network status information.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GetNetworkStatusInformationResponse : IResponse
{
    /// <summary>
    /// Gets or initializes the network status information if the request was successful.
    /// </summary>
    public NetworkStatusInformation? NetworkStatusInformation { get; init; }

    /// <summary>
    /// Gets or initializes error information if the request failed, otherwise <see langword="null"/>.
    /// </summary>
    public ErrorInfo? RequestError { get; init; }
}
