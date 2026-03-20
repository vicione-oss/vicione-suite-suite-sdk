using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestRequestConsumer : RequestConsumer<TestConsumerRequest, TestConsumerResponse>
{
    /// <inheritdoc/>
    public override Task<TestConsumerResponse> Respond(TestConsumerRequest message, CancellationToken cancellationToken)
        => message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.FromResult(new TestConsumerResponse(message.RequestId));

    /// <inheritdoc/>
    public override Task<TestConsumerResponse> HandleException(TestConsumerRequest message, Exception e, CancellationToken cancellationToken)
        => Task.FromResult(new TestConsumerResponse(message.RequestId, new ErrorInfo(100, "Fail")));
}

public record TestConsumerRequest(Guid RequestId) : IRequest<TestConsumerResponse>
{
    public bool ThrowException { get; init; }
}

public record TestConsumerResponse(Guid RequestId, ErrorInfo? RequestError = null) : IResponse;
