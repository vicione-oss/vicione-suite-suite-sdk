using MassTransit;
using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace Sdk.Testing.Tests.Backend;

[MessageEndpoint("delayed-activity")]
internal sealed record DelayedActivityArguments : IActivityArgument
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    // Time the activity spends without bus activity, like a database or file access.
    public int DelayMilliseconds { get; init; }

    public bool ThrowException { get; init; }
}

internal sealed class DelayedActivity : IExecuteActivity<DelayedActivityArguments>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<DelayedActivityArguments> context)
    {
        await Task.Delay(context.Arguments.DelayMilliseconds, context.CancellationToken);

        if (context.Arguments.ThrowException)
            throw new InvalidOperationException("Delayed failure");

        return context.Completed();
    }
}
