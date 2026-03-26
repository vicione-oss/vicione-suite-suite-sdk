using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

/// <summary>
/// Represents an event indicating that a connection has been created, updated, or deleted.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record ConnectionChanged(CrudAction Action, Connection Connection, List<Tag> AddedTags, List<Tag> RemovedTags) : ResponseEventBase;
