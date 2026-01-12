using MassTransit;
using Sdk.Messaging;

namespace TestModule.Backend.Consumer;

public sealed class TestCommandConsumer : IConsumer<TestConsumerCommand>, IConsumer<Fault<TestConsumerCommand>>
{
    public Task Consume(ConsumeContext<TestConsumerCommand> context)
    {
        if (context.Message.ThrowException)
            throw new InvalidOperationException("TriggeredException");

        if (context.Message.CorrelationId == Guid.Empty)
            throw new InvalidOperationException("CorrelationIdEmpty");

        return context.Message.FireEvent
            ? context.Publish(new TestConsumerEvent(context.Message.CorrelationId))
            : Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<Fault<TestConsumerCommand>> context)
    {
        var faultEvent = new TestConsumerFaultEvent(
            context.Message.Message.CorrelationId,
            new(100, context.Message.Exceptions.First().Message));

        return context.Publish(faultEvent);
    }
}

public record TestConsumerCommand : ICommand
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    public bool ThrowException { get; init; }

    public bool FireEvent { get; init; }
}
