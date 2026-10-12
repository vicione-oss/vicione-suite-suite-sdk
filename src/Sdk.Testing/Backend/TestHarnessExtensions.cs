using System.Text;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.TestFramework;
using MassTransit.Testing;
using NUnit.Framework;
using Sdk.Backend.Messaging;
using Sdk.Messaging;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="ITestHarness"/> to simplify common messaging test scenarios.
/// </summary>
/// <remarks>
/// The helpers return once the consumers registered in the harness have handled the message and every message it caused,
/// such as its <see cref="Fault{T}"/> and the events of a fault consumer. They wait at most <see cref="IBaseTestHarness.TestTimeout"/>.
/// Afterwards the harness's <c>Published</c>, <c>Sent</c> and <c>Consumed</c> return what is recorded without waiting for more.
/// </remarks>
public static class TestHarnessExtensions
{
    extension(ITestHarness harness)
    {
        /// <summary>
        /// Sends a request and waits for a response, asserting that the request was consumed and the response was sent.
        /// </summary>
        /// <exception cref="RequestFaultException">Thrown by the request client if the consumer faulted, before any assertion.</exception>
        /// <exception cref="RequestTimeoutException">Thrown if no response arrives within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        /// <exception cref="TimeoutException">Thrown if the request was not handled within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        public async Task<TResponse> TestRequest<TResponse, TRequest>(TRequest request)
            where TRequest : class, IRoutableMessage
            where TResponse : class, IResponse
        {
            await harness.Start();

            var client = harness.Bus.CreateRequestClient<TRequest>();
            Response<TResponse>? response = null;

            // The request client's own default of 30 s would end a debugging session at a breakpoint before TestTimeout does.
            await harness.SendAndWait<TRequest>(async stamp => response = await client.GetResponse<TResponse>(request,
                x => x.UseExecute(stamp),
                harness.CancellationToken,
                harness.TestTimeout));

            Assert.That(await harness.Consumed.Any<TRequest>(harness.CancellationToken));

            await harness.ThrowOnConsumeFault<TRequest>();

            // MassTransit records a consumer exception instead of propagating it, so it is rethrown here.
            await harness.ThrowOnConsumeError<TRequest>();

            Assert.That(await harness.Sent.Any<TResponse>(harness.CancellationToken));

            return response!.Message;
        }

        /// <summary>
        /// Sends a command to the endpoint of <typeparamref name="TConsumer"/> and asserts that the consumer consumed it.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="correlationId">
        /// Sets the send context's correlation ID, as <c>UiCore.Backend.MessageHub</c> does for a command from the frontend;
        /// <see langword="null"/> leaves it to MassTransit.
        /// </param>
        /// <param name="throwFirstFaultException">
        /// <see langword="true"/> turns a published <see cref="Fault{T}"/> into an <see cref="InvalidOperationException"/>;
        /// <see langword="false"/> lets the consumer's original exception propagate instead.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a fault was published and <paramref name="throwFirstFaultException"/> is set.
        /// </exception>
        /// <exception cref="TimeoutException">Thrown if the command was not handled within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        public async Task TestCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null, bool throwFirstFaultException = true)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
        {
            // The DI container stops the harness on dispose, so there is no matching Stop().
            await harness.Start();

            // Act
            var endpoint = await harness.GetConsumerEndpoint<TConsumer>();

            await harness.SendAndWait<TCommand>(stamp => correlationId is null
                    ? endpoint.Send(command, context => stamp(context), harness.CancellationToken)
                    : endpoint.Send(command,
                        command.GetType(),
                        context =>
                        {
                            stamp(context);
                            context.CorrelationId = correlationId;
                        },
                        harness.CancellationToken));

            if (throwFirstFaultException)
                await harness.ThrowOnConsumeFault<TCommand>();

            var consumerHarness = harness.GetConsumerHarness<TConsumer>();
            Assert.That(await consumerHarness.Consumed.Any<TCommand>(harness.CancellationToken));

            await harness.ThrowOnConsumeError<TCommand>();
        }

        /// <summary>
        /// Sends a command like <see cref="TestCommand{TCommand, TConsumer}"/> and returns the first published
        /// <typeparamref name="TResponseEvent"/>, asserting that one was published.
        /// </summary>
        public async Task<TResponseEvent> TestCommand<TCommand, TConsumer, TResponseEvent>(TCommand command)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
            where TResponseEvent : class
        {
            await harness.TestCommand<TCommand, TConsumer>(command);

            // Assert
            Assert.That(await harness.Published.Any<TResponseEvent>(harness.CancellationToken));
            return (await harness.Published.SelectAsync<TResponseEvent>(harness.CancellationToken).First()).Context.Message;
        }

        /// <summary>
        /// Sends <paramref name="commands"/> in order, asserting each was consumed, and returns every
        /// <typeparamref name="TResponseEvent"/> the harness saw published.
        /// </summary>
        /// <param name="commands">The commands to send, one after another; each is handled before the next is sent.</param>
        /// <param name="throwFirstFaultException">
        /// <see langword="true"/> turns a published <see cref="Fault{T}"/> into an <see cref="InvalidOperationException"/>;
        /// <see langword="false"/> lets the consumer's original exception propagate instead.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a fault was published and <paramref name="throwFirstFaultException"/> is set.
        /// </exception>
        /// <exception cref="TimeoutException">Thrown if a command was not handled within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        public async Task<TResponseEvent[]> TestCommands<TCommand, TConsumer, TResponseEvent>(TCommand[] commands, bool throwFirstFaultException = true)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
            where TResponseEvent : class
        {
            // The DI container stops the harness on dispose, so there is no matching Stop().
            await harness.Start();

            var endpoint = await harness.GetConsumerEndpoint<TConsumer>();
            var consumerHarness = harness.GetConsumerHarness<TConsumer>();

            foreach (var command in commands)
            {
                // Act
                await harness.SendAndWait<TCommand>(stamp => endpoint.Send(command, context => stamp(context), harness.CancellationToken));

                if (throwFirstFaultException)
                    await harness.ThrowOnConsumeFault<TCommand>();

                Assert.That(await consumerHarness.Consumed.Any<TCommand>(harness.CancellationToken));

                await harness.ThrowOnConsumeError<TCommand>();
            }

            // Assert
            Assert.That(await harness.Published.Any<TResponseEvent>(harness.CancellationToken));
            return [.. (await harness.Published.SelectAsync<TResponseEvent>(harness.CancellationToken).ToListAsync()).Select(k => k.Context.Message)];
        }

        /// <summary>
        /// Publishes an event and asserts that <typeparamref name="TConsumer"/> consumed it.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown if the event was not handled within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        public async Task TestEvent<TEvent, TConsumer>(TEvent @event)
            where TEvent : class, IRoutableMessage
            where TConsumer : class, IConsumer
        {
            // The DI container stops the harness on dispose, so there is no matching Stop().
            await harness.Start();

            // Act
            await harness.SendAndWait<TEvent>(stamp => harness.Bus.Publish(@event, context => stamp(context), harness.CancellationToken));

            await harness.ThrowOnConsumeError<TEvent>();

            Assert.That(await harness.Published.Any<TEvent>(harness.CancellationToken));
            var consumerHarness = harness.GetConsumerHarness<TConsumer>();
            Assert.That(await consumerHarness.Consumed.Any<TEvent>(harness.CancellationToken));
        }

        /// <summary>
        /// Throws if a <see cref="Fault{T}"/> of <typeparamref name="TMessage"/> was published; returns otherwise.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a fault was published; the message carries the first exception's message and, if present,
        /// its inner exception's message and stack trace.
        /// </exception>
        public async Task ThrowOnConsumeFault<TMessage>()
            where TMessage : class, IRoutableMessage
        {
            if (!await harness.Published.Any<Fault<TMessage>>(harness.CancellationToken))
                return;

            var consumed = await harness.Published.SelectAsync<Fault<TMessage>>(harness.CancellationToken).ToListAsync();
            var receivedMessage = consumed.FirstOrDefault(k => k.Context.Message.Exceptions.Length > 0);
            if (receivedMessage is null)
                return;

            // A fault can carry several exceptions; only the first is reported.
            var exceptionInfo = receivedMessage.Context.Message.Exceptions[0];
            if (exceptionInfo.InnerException is null)
                throw new InvalidOperationException(exceptionInfo.Message);

            var sb = new StringBuilder();
            sb.AppendLine(exceptionInfo.Message);
            sb.AppendLine(exceptionInfo.InnerException.Message);
            sb.AppendLine(exceptionInfo.InnerException.StackTrace);

            throw new InvalidOperationException(sb.ToString());
        }

        /// <summary>
        /// Rethrows the first exception a consumer of <typeparamref name="TMessage"/> threw, whatever its type;
        /// MassTransit records it on the received message instead of propagating it.
        /// </summary>
        private async Task ThrowOnConsumeError<TMessage>()
            where TMessage : class, IRoutableMessage
        {
            var consumed = await harness.Consumed.SelectAsync<TMessage>(harness.CancellationToken).ToListAsync();
            var receivedMessage = consumed.FirstOrDefault(k => k.Exception is not null);
            if (receivedMessage is not null)
                throw receivedMessage.Exception;
        }

        /// <summary>
        /// Runs <typeparamref name="T"/> as the only activity of a routing slip and waits until the activity has handled it.
        /// Nothing is asserted: the caller checks the outcome, e.g. via the harness's published messages.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown if the activity did not finish within <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        public async Task TestExecuteActivity<T, TArguments>(TArguments arguments,
            IEnumerable<KeyValuePair<string, object>>? variables = null)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class, IActivityArgument
        {
            // The DI container stops the harness on dispose, so there is no matching Stop().
            await harness.Start();

            // Act
            var address = new Uri($"queue:{harness.EndpointNameFormatter.ExecuteActivity<T, TArguments>()}");

            // MassTransit runs an activity only as part of a routing slip, so a one-activity slip is built around it.
            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            builder.AddActivity("Testo", address, arguments);

            if (variables is not null)
                builder.SetVariables(variables);

            // Sending the slip to its only activity is what Bus.Execute does; sending it here sets the MessageId to wait for.
            var routingSlip = builder.Build();
            var endpoint = await harness.Bus.GetSendEndpoint(address);
            await harness.SendAndWait<RoutingSlip>(stamp => endpoint.Send(routingSlip, context => stamp(context), harness.CancellationToken),
                address);
        }

        /// <summary>
        /// Sends one message, which <paramref name="send"/> passes through the given callback, and waits until the harness's
        /// consumers have handled it and the messages it caused; <paramref name="targetAddress"/> names an endpoint no consumer
        /// registration covers.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown if that takes longer than <see cref="IBaseTestHarness.TestTimeout"/>.</exception>
        private async Task SendAndWait<TMessage>(Func<Action<SendContext>, Task> send, Uri? targetAddress = null)
            where TMessage : class
        {
            var observer = new ReceiveCompletionObserver(harness, targetAddress);

            using (harness.Bus.ConnectReceiveObserver(observer))
            using (harness.Bus.ConnectSendObserver(observer))
            using (harness.Bus.ConnectPublishObserver(observer))
            {
                try
                {
                    await send(observer.Stamp);
                    observer.SendCompleted();

                    await observer.Completed.WaitAsync(harness.TestTimeout, harness.CancellationToken);
                }
                // The harness's token runs on TestTimeout from Start() on and may expire first, even during the send.
                catch (Exception exception) when (exception is TimeoutException
                    || (exception is OperationCanceledException && harness.CancellationToken.IsCancellationRequested))
                {
                    throw new TimeoutException($"{typeof(TMessage).Name} was not handled within {harness.TestTimeout}.", exception);
                }
            }

            // Ends the harness's inactivity wait, so its message lists return what is recorded instead of waiting for more.
            harness.ForceInactive();
        }
    }

    extension(InMemoryTestHarness harness)
    {
        /// <summary>
        /// Executes an activity and waits for <see cref="RoutingSlipActivityCompleted"/>, returning the context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the completed routing slip has a different tracking number.</exception>
        public async Task<ConsumeContext<RoutingSlipActivityCompleted>> TestExecuteActivityCompletedWithVariables<TArguments>(ActivityTestContext activity,
            TArguments arguments,
            IEnumerable<KeyValuePair<string, object>>? variables = null)
            where TArguments : class
        {
            var completed = harness.SubscribeHandler<RoutingSlipCompleted>();
            var activityCompleted = harness.SubscribeHandler<RoutingSlipActivityCompleted>();

            var argumentsDictionary = RoutingSlipBuilder.GetObjectAsDictionary(arguments);

            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(harness.BusAddress, RoutingSlipEvents.All);
            builder.AddActivity(activity.Name, activity.ExecuteUri, arguments);
            builder.AddCompensateLog(trackingNumber, activity.ExecuteUri, argumentsDictionary);

            if (variables is not null)
                builder.SetVariables(variables);

            await harness.Bus.Execute(builder.Build());

            await completed;
            var context = await activityCompleted!;

            // The routing slip has completed, so the message lists need not wait for more.
            harness.ForceInactive();

            if (!Equals(trackingNumber, context.Message.TrackingNumber))
                throw new InvalidOperationException("Tracking number of the routing slip does not match");

            return context;
        }

        /// <summary>
        /// Executes an activity and waits for <see cref="RoutingSlipActivityCompleted"/>.
        /// </summary>
        public Task TestExecuteActivityCompleted<TArguments>(ActivityTestContext activity, TArguments arguments)
            where TArguments : class, IActivityArgument
            => harness.TestExecuteActivityCompletedWithVariables(activity, arguments);

        /// <summary>
        /// Executes an activity and waits for <see cref="RoutingSlipActivityFaulted"/>, returning the context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the faulted routing slip has a different tracking number.</exception>
        public async Task<ConsumeContext<RoutingSlipActivityFaulted>> TestExecuteActivityFaultedWithVariables<TArguments>(ActivityTestContext activity,
            TArguments arguments)
            where TArguments : class, IActivityArgument
        {
            var faulted = harness.SubscribeHandler<RoutingSlipFaulted>();
            var activityFaulted = harness.SubscribeHandler<RoutingSlipActivityFaulted>();

            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(harness.BusAddress, RoutingSlipEvents.All);
            builder.AddActivity(activity.Name, activity.ExecuteUri, arguments);

            await harness.Bus.Execute(builder.Build());

            await faulted;
            var context = await activityFaulted!;

            // The routing slip has faulted, so the message lists need not wait for more.
            harness.ForceInactive();

            if (!Equals(trackingNumber, context.Message.TrackingNumber))
                throw new InvalidOperationException("Tracking number of the routing slip does not match");

            return context;
        }

        /// <summary>
        /// Executes an activity and waits for <see cref="RoutingSlipActivityFaulted"/>.
        /// </summary>
        public Task TestExecuteActivityFaulted<TArguments>(ActivityTestContext activity, TArguments arguments)
            where TArguments : class, IActivityArgument
            => harness.TestExecuteActivityFaultedWithVariables(activity, arguments);
    }
}
