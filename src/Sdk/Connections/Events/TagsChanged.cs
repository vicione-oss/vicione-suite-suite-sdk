using Sdk.Connections.Contracts;
using Sdk.Messaging;

namespace Sdk.Connections.Events;

/// <summary>
/// Represents an event that is published when one or more tags have been created, updated, or deleted.
/// </summary>
[ForwardToUI]
[ExcludeFromCodeCoverage]
public sealed record TagsChanged(CrudAction Action, List<Tag> Tags) : ResponseEventBase;
