using MassTransit;
using MassTransit.Configuration;
using MassTransit.Testing;
using Sdk.Backend.Messaging;

namespace Sdk.Testing.Backend;

/// <summary>
/// Completes once the harness's endpoints have received one message and every message of its conversation that they
/// receive, such as a consumer's <see cref="Fault{T}"/>, its handling by a fault consumer and the events that follow.
/// </summary>
/// <remarks>
/// MassTransit passes the <c>ConversationId</c> on to every message sent from a consume context. A message counts as
/// expected at the endpoint it is sent to, or, when published, at every consumer endpoint registered in the harness that
/// consumes its type. Work a consumer starts outside its consume context, e.g. in <see cref="Task.Run(Func{Task})"/>, starts a
/// new conversation and is not followed; neither are sagas.
/// </remarks>
internal sealed class ReceiveCompletionObserver : IReceiveObserver, ISendObserver, IPublishObserver
{
    private readonly Guid _messageId = NewId.NextGuid();
    private readonly Guid _conversationId = NewId.NextGuid();
    private readonly HashSet<string> _endpoints;
    private readonly (string Endpoint, Type[] MessageTypes)[] _consumerEndpoints;
    private readonly HashSet<(Guid MessageId, string Endpoint)> _pending = [];
    private readonly TaskCompletionSource _completed = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Completed => _completed.Task;

    /// <summary>
    /// Follows the message that <see cref="Stamp"/> marks through the endpoints of <paramref name="harness"/>, once the caller has
    /// connected the observer to the bus.
    /// </summary>
    /// <param name="harness">Provides the consumer registrations and the bus endpoint.</param>
    /// <param name="targetAddress">An endpoint the message is sent to that no consumer registration names, e.g. an activity's.</param>
    public ReceiveCompletionObserver(ITestHarness harness, Uri? targetAddress = null)
    {
        var context = harness.Provider.GetRequiredService<IBusRegistrationContext>();

        _consumerEndpoints = [.. harness.Provider.GetServices<IConsumerRegistration>()
            .Where(registration => registration.IncludeInConfigureEndpoints)
            .Select(registration => (
                registration.GetDefinition(context).GetEndpointName(harness.EndpointNameFormatter),
                registration.Type.FindAllMessageTypes().ToArray()))];

        _endpoints = [.. _consumerEndpoints.Select(x => x.Endpoint), GetEndpointName(harness.Bus.Address)];
        if (targetAddress is not null)
            _endpoints.Add(GetEndpointName(targetAddress));
    }

    /// <summary>
    /// Marks the message to follow; call it from the send pipe of exactly one message.
    /// </summary>
    public void Stamp(SendContext context)
    {
        context.MessageId = _messageId;
        context.ConversationId = _conversationId;
    }

    /// <summary>
    /// Completes <see cref="Completed"/> if the message went nowhere this observer waits for; call it once the send returned.
    /// </summary>
    public void SendCompleted()
    {
        lock (_pending)
        {
            if (_pending.Count == 0)
                _completed.TrySetResult();
        }
    }

    public Task PreSend<T>(SendContext<T> context)
        where T : class
    {
        if (context.DestinationAddress is { } address && _endpoints.Contains(GetEndpointName(address)))
            Track(context, [GetEndpointName(address)]);

        return Task.CompletedTask;
    }

    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        var messageType = context.Message.GetType();
        Track(context, _consumerEndpoints
            .Where(x => x.MessageTypes.Any(t => t.IsAssignableFrom(messageType)))
            .Select(x => x.Endpoint));

        return Task.CompletedTask;
    }

    public Task PostReceive(ReceiveContext context)
        => Received(context);

    public Task ReceiveFault(ReceiveContext context, Exception exception)
        => Received(context);

    public Task PostSend<T>(SendContext<T> context)
        where T : class
        => Task.CompletedTask;

    public Task SendFault<T>(SendContext<T> context, Exception exception)
        where T : class
        => Task.CompletedTask;

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class
        => Task.CompletedTask;

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
        => Task.CompletedTask;

    public Task PreReceive(ReceiveContext context)
        => Task.CompletedTask;

    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        where T : class
        => Task.CompletedTask;

    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        where T : class
        => Task.CompletedTask;

    // Runs before the transport hands the message to a queue, so it is pending before its receive can finish.
    private void Track(SendContext context, IEnumerable<string> endpoints)
    {
        if (context.ConversationId != _conversationId || context.MessageId is not { } messageId)
            return;

        lock (_pending)
        {
            foreach (var endpoint in endpoints)
                _pending.Add((messageId, endpoint));
        }
    }

    private Task Received(ReceiveContext context)
    {
        if (context.GetMessageId() is not { } messageId)
            return Task.CompletedTask;

        lock (_pending)
        {
            if (_pending.Remove((messageId, GetEndpointName(context.InputAddress))) && _pending.Count == 0)
                _completed.TrySetResult();
        }

        return Task.CompletedTask;
    }

    // Covers both the short form (queue:name) and the transport form (loopback://localhost/name?bind=true).
    private static string GetEndpointName(Uri address)
        => address.AbsolutePath.Split('/')[^1];
}
