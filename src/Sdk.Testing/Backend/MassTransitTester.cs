using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Messaging;
using Xunit.Abstractions;

namespace Sdk.Testing.Backend;

public sealed class MassTransitTester : IAsyncDisposable
{
    private readonly TestOutputHelperTextWriterAdapter? _testOutputHelperTextWriterAdapter;

    public ServiceProvider Services { get; }
    public ITestHarness Harness { get; }

    public MassTransitTester(Action<IBusRegistrationConfigurator>? servicesConfig = null, ITestOutputHelper? testOutputHelper = null)
    {
        var textWriter = testOutputHelper is null
            ? TextWriter.Null
            : _testOutputHelperTextWriterAdapter = new(testOutputHelper);

        Services = new ServiceCollection()
            .AddMassTransitTestHarness(textWriter, servicesConfig)
            .BuildServiceProvider(true);
        Harness = Services.GetRequiredService<ITestHarness>();
    }

    public async ValueTask DisposeAsync()
    {
        if (Harness is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else if (Harness is IDisposable disposable)
            disposable.Dispose();

        if (_testOutputHelperTextWriterAdapter is not null)
            await _testOutputHelperTextWriterAdapter.DisposeAsync();

        await Services.DisposeAsync();
    }

    public Task<TResponse> TestRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    public Task<TResponse> TestInstanceDependentRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IInstanceDependentRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    public Task TestCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    public Task TestCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    public Task TestInstanceDependentCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    public Task TestInstanceDependentCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    public Task<TResponseEvent> TestCommand<TCommand, TConsumer, TResponseEvent>(TCommand command)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommand<TCommand, TConsumer, TResponseEvent>(command);

    public Task<TResponseEvent[]> TestCommands<TCommand, TConsumer, TResponseEvent>(TCommand[] commands)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommands<TCommand, TConsumer, TResponseEvent>(commands);

    public Task TestEvent<TEvent, TConsumer>(TEvent @event)
        where TEvent : class, IEvent
        where TConsumer : class, IConsumer
        => Harness.TestEvent<TEvent, TConsumer>(@event);

    public Task TestExecuteActivity<T, TArguments>(TArguments arguments, IEnumerable<KeyValuePair<string, object>>? variables = null)
        where T : class, IExecuteActivity<TArguments>
        where TArguments : class, IActivityArgument
        => Harness.TestExecuteActivity<T, TArguments>(arguments, variables);
}
