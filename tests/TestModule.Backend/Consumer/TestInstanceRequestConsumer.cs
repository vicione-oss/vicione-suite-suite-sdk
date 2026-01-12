using MassTransit;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestInstanceRequestConsumer : InstanceDependentRequestConsumer<TestInstanceConsumerRequest, TestInstanceConsumerResponse>
{
    protected override Task<TestInstanceConsumerResponse> Respond(ConsumeContext<TestInstanceConsumerRequest> context)
        => context.Message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.FromResult<TestInstanceConsumerResponse>(new(context.Message.RequestId));

    protected override Task<TestInstanceConsumerResponse> HandleException(ConsumeContext<TestInstanceConsumerRequest> context, Exception e)
        => Task.FromResult<TestInstanceConsumerResponse>(new(context.Message.RequestId, new ErrorInfo(100, "Fail")));
}

public record TestInstanceConsumerRequest(Guid RequestId) : IInstanceDependentRequest<TestInstanceConsumerResponse>
{
    public bool ThrowException { get; init; }
}

public record TestInstanceConsumerResponse(Guid RequestId, ErrorInfo? RequestError = null) : IResponse;
