using Sdk.Messaging;

namespace Sdk.Connections.Requests;

/// <summary>
/// Represents a parameterless request to retrieve all available connection tags.
/// </summary>
public sealed record GetTags : IRequest<GetTagsResponse>;
