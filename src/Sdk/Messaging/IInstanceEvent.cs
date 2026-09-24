namespace Sdk.Messaging;

/// <summary>
/// An event consumed only on the instance that publishes it.
/// </summary>
public interface IInstanceEvent : IInstanceDependentMessage, IEvent;
