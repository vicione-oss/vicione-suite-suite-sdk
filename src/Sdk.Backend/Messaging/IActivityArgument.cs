using Sdk.Backend.Diagnostics;
using Sdk.Messaging;

namespace Sdk.Backend.Messaging;

/// <summary>
/// The arguments passed to a routing-slip activity.
/// </summary>
/// <remarks>
/// The endpoint name is fixed, so when several instances host the same activity, they all bind to
/// that single queue, and a message is delivered to whichever instance happens to be listening.
/// To target one specific instance instead, use <see cref="IInstanceDependentActivityArgument"/>.
/// </remarks>
[MustDeclare(typeof(MessageEndpointAttribute))]
public interface IActivityArgument : IRoutableMessage;
