using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

public sealed record GetTagsResponse(List<Tag> Tags, ErrorInfo? RequestError = null) : IResponse;
