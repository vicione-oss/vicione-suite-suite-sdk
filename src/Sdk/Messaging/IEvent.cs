namespace Sdk.Messaging;

/// <summary>
/// A message sent to the central broker and consumed by every instance that has a suitable consumer.
/// </summary>
public interface IEvent : IRoutableMessage;
