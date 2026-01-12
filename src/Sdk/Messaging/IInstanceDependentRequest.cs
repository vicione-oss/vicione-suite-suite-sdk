namespace Sdk.Messaging;

/// <summary>
/// A request that will fetch the result from a specific instance.
/// </summary>
public interface IInstanceDependentRequest<T> : IInstanceDependentMessage
    where T : IResponse;
