namespace Sdk.Messaging;

/// <summary>
/// Message that is sent to the central broker and is then consumed by any instance that has a suitable consumer (broadcast behavior)
/// </summary>
public interface IEvent : IRoutableMessage;
