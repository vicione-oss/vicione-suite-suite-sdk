using MassTransit;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestEventConsumer : IConsumer<TestConsumerEvent>
{
    public Task Consume(ConsumeContext<TestConsumerEvent> context)
        => context.Message.ThrowException
            ? throw new InvalidOperationException("TriggeredException")
            : Task.CompletedTask;
}

public record TestConsumerEvent(Guid CorrelationId) : IEvent
{
    public bool ThrowException { get; init; }
}

public record TestConsumerFaultEvent(Guid CorrelationId, ErrorInfo Info) : IEvent;
