using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestInstanceRequestConsumer : InstanceDependentRequestConsumer<TestInstanceConsumerRequest, TestInstanceConsumerResponse>
{
    /// <inheritdoc/>
    public override Task<TestInstanceConsumerResponse> Respond(TestInstanceConsumerRequest message, CancellationToken cancellationToken)
        => message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.FromResult(new TestInstanceConsumerResponse(message.RequestId));

    /// <inheritdoc/>
    public override Task<TestInstanceConsumerResponse> HandleException(TestInstanceConsumerRequest message, Exception e, CancellationToken cancellationToken)
        => Task.FromResult(new TestInstanceConsumerResponse(message.RequestId, new ErrorInfo(100, "Fail")));
}

public record TestInstanceConsumerRequest(Guid RequestId) : IInstanceDependentRequest<TestInstanceConsumerResponse>
{
    public bool ThrowException { get; init; }
}

public record TestInstanceConsumerResponse(Guid RequestId, ErrorInfo? RequestError = null) : IResponse;
