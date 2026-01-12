using MassTransit;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestInstanceEventConsumer : IConsumer<TestInstanceConsumerEvent>
{
    public Task Consume(ConsumeContext<TestInstanceConsumerEvent> context)
        => context.Message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.CompletedTask;
}

public record TestInstanceConsumerEvent(Guid CorrelationId) : IInstanceEvent
{
    public bool ThrowException { get; init; }
}

public record TestInstanceConsumerFaultEvent(Guid CorrelationId, ErrorInfo Info) : IInstanceEvent;
