using MassTransit;
using MassTransit.Testing;
using Sdk.Backend.Messaging;
using Sdk.Messaging;
using Xunit;

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
    /// Builds a service provider with the MassTransit test harness; bus output goes to <paramref name="testOutputHelper"/>.
    /// </summary>
    /// <param name="servicesConfig">Registers consumers, activities and sagas under test.</param>
    /// <param name="testOutputHelper">Receives the harness log; <see langword="null"/> discards it.</param>
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
    /// Sends a request and returns the response, asserting that the request was consumed and the response sent.
    /// </summary>
    /// <exception cref="RequestFaultException">Thrown by the request client if the consumer faulted, before any assertion.</exception>
    public Task<TResponse> TestRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    /// <summary>
    /// Sends an instance-dependent request and returns the response, asserting that the request was consumed and the response sent.
    /// </summary>
    /// <exception cref="RequestFaultException">Thrown by the request client if the consumer faulted, before any assertion.</exception>
    public Task<TResponse> TestInstanceDependentRequest<TResponse, TRequest>(TRequest request)
        where TResponse : class, IResponse
        where TRequest : class, IInstanceDependentRequest<TResponse>
        => Harness.TestRequest<TResponse, TRequest>(request);

    /// <summary>
    /// Sends a command to <typeparamref name="TConsumer"/> and asserts that it was consumed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the consumer faulted; carries the fault's first exception message.</exception>
    public Task TestCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    /// <summary>
    /// Like <see cref="TestCommand{TCommand, TConsumer}(TCommand, Guid?)"/>, but a consumer exception propagates as the
    /// original exception rather than an <see cref="InvalidOperationException"/> rebuilt from the published fault.
    /// </summary>
    public Task TestCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    /// <summary>
    /// Sends an instance-dependent command to <typeparamref name="TConsumer"/> and asserts that it was consumed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the consumer faulted; carries the fault's first exception message.</exception>
    public Task TestInstanceDependentCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId);

    /// <summary>
    /// Like <see cref="TestInstanceDependentCommand{TCommand, TConsumer}(TCommand, Guid?)"/>, but a consumer exception propagates
    /// as the original exception rather than an <see cref="InvalidOperationException"/> rebuilt from the published fault.
    /// </summary>
    public Task TestInstanceDependentCommandFault<TCommand, TConsumer>(TCommand command, Guid? correlationId = null)
        where TCommand : class, IInstanceDependentCommand
        where TConsumer : class, IConsumer
        => Harness.TestCommand<TCommand, TConsumer>(command, correlationId, false);

    /// <summary>
    /// Sends a command to <typeparamref name="TConsumer"/> and returns the first published <typeparamref name="TResponseEvent"/>,
    /// asserting that one was published.
    /// </summary>
    public Task<TResponseEvent> TestCommand<TCommand, TConsumer, TResponseEvent>(TCommand command)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommand<TCommand, TConsumer, TResponseEvent>(command);

    /// <summary>
    /// Sends the commands in order, asserting each was consumed, and returns every published <typeparamref name="TResponseEvent"/>.
    /// </summary>
    public Task<TResponseEvent[]> TestCommands<TCommand, TConsumer, TResponseEvent>(TCommand[] commands)
        where TCommand : class, ICommand
        where TConsumer : class, IConsumer
        where TResponseEvent : class
        => Harness.TestCommands<TCommand, TConsumer, TResponseEvent>(commands);

    /// <summary>
    /// Publishes an event and asserts that <typeparamref name="TConsumer"/> consumed it.
    /// </summary>
    public Task TestEvent<TEvent, TConsumer>(TEvent @event)
        where TEvent : class, IEvent
        where TConsumer : class, IConsumer
        => Harness.TestEvent<TEvent, TConsumer>(@event);

    /// <summary>
    /// Runs <typeparamref name="T"/> as the only activity of a routing slip; nothing is asserted, so the test checks the outcome.
    /// </summary>
    public Task TestExecuteActivity<T, TArguments>(TArguments arguments, IEnumerable<KeyValuePair<string, object>>? variables = null)
        where T : class, IExecuteActivity<TArguments>
        where TArguments : class, IActivityArgument
        => Harness.TestExecuteActivity<T, TArguments>(arguments, variables);
}
