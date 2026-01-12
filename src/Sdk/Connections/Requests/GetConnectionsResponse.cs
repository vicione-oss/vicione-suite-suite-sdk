using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

public sealed record GetConnectionsResponse(List<Connection> Connections, ErrorInfo? RequestError = null) : IResponse;
