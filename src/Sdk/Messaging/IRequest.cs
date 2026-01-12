namespace Sdk.Messaging;

/// <summary>
/// A message that is sent to the in-memory bus of a local instance. The message never leaves the instance
/// due to the desire to have the system work in read-only mode when the system is not connected to a cluster.
/// </summary>
/// <typeparam name="T">The response type</typeparam>
public interface IRequest<T> : IRoutableMessage where T : IResponse;
