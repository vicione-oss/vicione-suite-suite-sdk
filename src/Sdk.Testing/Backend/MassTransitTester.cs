using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Messaging;
using Xunit.Abstractions;

namespace Sdk.Testing.Backend;

/// <summary>
/// A wrapper class that simplifies the use of MassTransit's <see cref="ITestHarness"/> for common testing scenarios.
/// </summary>
public sealed class MassTransitTester : IAsyncDisposable
{
    private readonly TestOutputHelperTextWriterAdapter? _testOutputHelperTextWriterAdapter;

    /// <summary>
    /// Gets the configured <see cref="ServiceProvider"/> for the test environment.
    /// </summary>
    public ServiceProvider Services { get; }

    /// <summary>
    /// Gets the underlying <see cref="ITestHarness"/> instance for advanced test scenarios.
    /// </summary>
    public ITestHarness Harness { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MassTransitTester"/> class.
    /// </summary>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Tests a request-response message exchange.
    /// </summary>
    public Task<TResponse> TestRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    /// <summary>
    /// Tests an instance-dependent request-response message exchange.
    /// </summary>
    public Task<TResponse> TestInstanceDependentRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IInstanceDependentRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    /// <summary>
    /// Tests a command sent to a specific consumer.
    /// </summary>
    public Task TestCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    /// <summary>
    /// Tests a command sent to a specific consumer, expecting a potential fault.
    /// </summary>
    public Task TestCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    /// <summary>
    /// Tests an instance-dependent command sent to a specific consumer.
    /// </summary>
    public Task TestInstanceDependentCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    /// <summary>
    /// Tests an instance-dependent command sent to a specific consumer, expecting a potential fault.
    /// </summary>
    public Task TestInstanceDependentCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    /// <summary>
    /// Tests a command that is expected to publish a specific event as a result.
    /// </summary>
    public Task<TResponseEvent> TestCommand<TCommand, TConsumer, TResponseEvent>(TCommand command)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommand<TCommand, TConsumer, TResponseEvent>(command);

    /// <summary>
    /// Tests a series of commands and collects the resulting published events.
    /// </summary>
    public Task<TResponseEvent[]> TestCommands<TCommand, TConsumer, TResponseEvent>(TCommand[] commands)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommands<TCommand, TConsumer, TResponseEvent>(commands);

    /// <summary>
    /// Tests an event published to a specific consumer.
    /// </summary>
    public Task TestEvent<TEvent, TConsumer>(TEvent @event)
        where TEvent : class, IEvent
        where TConsumer : class, IConsumer
        => Harness.TestEvent<TEvent, TConsumer>(@event);

    /// <summary>
    /// Tests the execution of a routing slip activity.
    /// </summary>
    public Task TestExecuteActivity<T, TArguments>(TArguments arguments, IEnumerable<KeyValuePair<string, object>>? variables = null)
        where T : class, IExecuteActivity<TArguments>
        where TArguments : class, IActivityArgument
        => Harness.TestExecuteActivity<T, TArguments>(arguments, variables);
}
