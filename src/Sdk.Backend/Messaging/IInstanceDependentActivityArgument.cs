using Sdk.Backend.Diagnostics;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// The arguments passed to a routing-slip activity that runs on a specific instance.
/// </summary>
/// <remarks>
/// The endpoint name includes the instance identifier, so the message is routed to that exact
/// instance rather than any instance listening on the shared queue.
/// To target any available instance instead, use <see cref="IActivityArgument"/>.
/// </remarks>
[MustDeclare(typeof(MessageEndpointAttribute))]
public interface IInstanceDependentActivityArgument : IInstanceDependentMessage;
