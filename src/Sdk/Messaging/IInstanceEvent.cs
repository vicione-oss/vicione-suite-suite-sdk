namespace Sdk.Messaging;

/// <summary>
/// An event that will be consumed only on the current instance
/// </summary>
public interface IInstanceEvent : IInstanceDependentMessage, IEvent;
