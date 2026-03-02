using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

/// <summary>
/// Represents a request to retrieve a list of connections, with optional filters.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GetConnections(Guid? ConnectionId, List<string>? FilterTypeNames, List<Tag>? RequiredTags = null, Guid? InstanceId = null)
    : IRequest<GetConnectionsResponse>;
