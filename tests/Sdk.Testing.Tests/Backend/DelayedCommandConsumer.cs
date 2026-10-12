using MassTransit;
using Sdk.Messaging;

namespace Sdk.Testing.Tests.Backend;

internal sealed record DelayedCommand : ICommand
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    // Time the consumer and its fault handling spend without bus activity, like a database or file access.
    public int DelayMilliseconds { get; init; }

    public bool ThrowException { get; init; }
}

internal sealed record DelayedCommandHandled(Guid CorrelationId) : IEvent;

internal sealed record DelayedCommandFailed(Guid CorrelationId) : IEvent;

internal sealed class DelayedCommandConsumer : IConsumer<DelayedCommand>, IConsumer<Fault<DelayedCommand>>
{
    public async Task Consume(ConsumeContext<DelayedCommand> context)
    {
        await Task.Delay(context.Message.DelayMilliseconds, context.CancellationToken);

        if (context.Message.ThrowException)
            throw new InvalidOperationException("Delayed failure");

        await context.Publish(new DelayedCommandHandled(context.Message.CorrelationId), context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<Fault<DelayedCommand>> context)
    {
        await Task.Delay(context.Message.Message.DelayMilliseconds, context.CancellationToken);
        await context.Publish(new DelayedCommandFailed(context.Message.Message.CorrelationId), context.CancellationToken);
    }
}

// Consumes the event on its own endpoint, like a fault consumer or another module's consumer.
internal sealed class DelayedCommandHandledConsumer : IConsumer<DelayedCommandHandled>
{
    public Task Consume(ConsumeContext<DelayedCommandHandled> context)
        => Task.Delay(200, context.CancellationToken);
}
