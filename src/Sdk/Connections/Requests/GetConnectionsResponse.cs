using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

/// <summary>
/// Represents the response to a <see cref="GetConnections"/> request.
/// </summary>
public sealed record GetConnectionsResponse(List<Connection> Connections, ErrorInfo? RequestError = null) : IResponse;
