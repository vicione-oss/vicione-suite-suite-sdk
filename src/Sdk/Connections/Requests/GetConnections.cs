using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

public sealed record GetConnections(Guid? ConnectionId, List<string>? FilterTypeNames, List<Tag>? RequiredTags = null, Guid? InstanceId = null)
    : IRequest<GetConnectionsResponse>;
