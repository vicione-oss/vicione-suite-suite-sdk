using MassTransit;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestRequestConsumer : RequestConsumer<TestConsumerRequest, TestConsumerResponse>
{
    protected override Task<TestConsumerResponse> Respond(ConsumeContext<TestConsumerRequest> context)
        => context.Message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.FromResult<TestConsumerResponse>(new(context.Message.RequestId));

    protected override Task<TestConsumerResponse> HandleException(ConsumeContext<TestConsumerRequest> context, Exception e)
        => Task.FromResult<TestConsumerResponse>(new(context.Message.RequestId, new ErrorInfo(100, "Fail")));
}

public record TestConsumerRequest(Guid RequestId) : IRequest<TestConsumerResponse>
{
    public bool ThrowException { get; init; }
}

public record TestConsumerResponse(Guid RequestId, ErrorInfo? RequestError = null) : IResponse;
