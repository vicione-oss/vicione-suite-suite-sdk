using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Requests;

/// <summary>
/// Represents the response to a request for all connection tags.
/// </summary>
public sealed record GetTagsResponse(List<Tag> Tags, ErrorInfo? RequestError = null) : IResponse;
