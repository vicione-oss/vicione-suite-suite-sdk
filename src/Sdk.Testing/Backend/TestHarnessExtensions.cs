using System.Text;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.TestFramework;
using MassTransit.Testing;
using NUnit.Framework;
using Sdk.Messaging;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="ITestHarness"/> to simplify common messaging test scenarios.
/// </summary>
public static class TestHarnessExtensions
{
    // This is the time the test harness waits for inactivity, before considering an operation completed
    private const int InactivityTimeout = 1;

    // This is the time the test can take for a consume
    private const int TestTimeout = 10;

    // On the system tests of ClusterManagement it was the case that we had to add a delay after
    // starting the harness to run all tests successful in one rush
    private const int StartDelay = 500;

    extension(ITestHarness harness)
    {
        /// <summary>
        /// Sends a request and waits for a response, asserting that the request was consumed and the response was sent.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a fault occurs during message consumption.</exception>
        public async Task<TResponse> TestRequest<TResponse, TRequest>(TRequest request)
            where TRequest : class, IRoutableMessage
            where TResponse : class, IResponse
        {
#if DEBUG
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);
#endif

            await harness.Start();
            await Task.Delay(StartDelay, harness.CancellationToken);

            var client = harness.Bus.CreateRequestClient<TRequest>();
            var response = await client.GetResponse<TResponse>(request, harness.CancellationToken);

            // did any endpoint consume the message
            Assert.That(await harness.Consumed.Any<TRequest>(harness.CancellationToken));

            await harness.InactivityTask;
            await harness.ThrowOnConsumeFault<TRequest>();

            // if an error occurred it gets swallowed by masstransit - throw it on the tester
            await harness.ThrowOnConsumeError<TRequest>();

            // assert that the response get sent
            Assert.That(await harness.Sent.Any<TResponse>(harness.CancellationToken));

            return response.Message;
        }

        /// <summary>
        /// Sends a command to a specific consumer and asserts that the command was consumed.
        /// </summary>
        public async Task TestCommand<TCommand, TConsumer>(TCommand command, Guid? correlationId = null, bool throwFirstFaultException = true)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
        {
            // di harness is shut down automatically
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);
            await harness.Start();
            await Task.Delay(StartDelay, harness.CancellationToken);

            // Act
            var endpoint = await harness.GetConsumerEndpoint<TConsumer>();

            if (correlationId is null)
            {
                await endpoint.Send(command, harness.CancellationToken);
            }
            else // this is what UiCore.Backend.MessageHub does to enrich the context with id from frontend
            {
                await endpoint.Send(command,
                    command.GetType(),
                    context => context.CorrelationId = correlationId,
                    harness.CancellationToken);
            }

            await harness.InactivityTask;

            // check if fault message was created
            if (throwFirstFaultException)
                await harness.ThrowOnConsumeFault<TCommand>();

            // did any endpoint consume the message
            var consumerHarness = harness.GetConsumerHarness<TConsumer>();
            Assert.That(await consumerHarness.Consumed.Any<TCommand>(harness.CancellationToken));

            await harness.ThrowOnConsumeError<TCommand>();
        }

        /// <summary>
        /// Sends a command and waits for a specific event to be published as a result.
        /// </summary>
        public async Task<TResponseEvent> TestCommand<TCommand, TConsumer, TResponseEvent>(TCommand command)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
            where TResponseEvent : class
        {
            await harness.TestCommand<TCommand, TConsumer>(command);

            await harness.InactivityTask;

            // Assert
            Assert.That(await harness.Published.Any<TResponseEvent>(harness.CancellationToken));
            return (await harness.Published.SelectAsync<TResponseEvent>(harness.CancellationToken).First()).Context.Message;
        }

        /// <summary>
        /// Sends a series of commands and collects the resulting published events.
        /// </summary>
        public async Task<TResponseEvent[]> TestCommands<TCommand, TConsumer, TResponseEvent>(TCommand[] commands, bool throwFirstFaultException = true)
            where TCommand : class, IRoutableMessage
            where TConsumer : class, IConsumer
            where TResponseEvent : class
        {
            // di harness is shut down automatically
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);
            await harness.Start();
            await Task.Delay(StartDelay, harness.CancellationToken);

            var endpoint = await harness.GetConsumerEndpoint<TConsumer>();
            var consumerHarness = harness.GetConsumerHarness<TConsumer>();

            // the deployment will be completed if we receive this one        
            foreach (var command in commands)
            {
                // Act
                await endpoint.Send(command, harness.CancellationToken);

                // check if fault message was created
                if (throwFirstFaultException)
                    await harness.ThrowOnConsumeFault<TCommand>();

                // did any endpoint consume the message            
                Assert.That(await consumerHarness.Consumed.Any<TCommand>(harness.CancellationToken));

                await harness.ThrowOnConsumeError<TCommand>();
            }

            await harness.InactivityTask;

            // Assert
            Assert.That(await harness.Published.Any<TResponseEvent>(harness.CancellationToken));
            return [.. (await harness.Published.SelectAsync<TResponseEvent>(harness.CancellationToken).ToListAsync()).Select(k => k.Context.Message)];
        }

        /// <summary>
        /// Publishes an event and asserts that it was consumed by a specific consumer.
        /// </summary>
        public async Task TestEvent<TEvent, TConsumer>(TEvent @event)
            where TEvent : class, IRoutableMessage
            where TConsumer : class, IConsumer
        {
            // di harness is shut down automatically
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);
            await harness.Start();
            await Task.Delay(StartDelay, harness.CancellationToken);

            // Act
            await harness.Bus.Publish(@event, harness.CancellationToken);
            await harness.InactivityTask;

            // did any endpoint consume the message
            Assert.That(await harness.Published.Any<TEvent>(harness.CancellationToken));
            var consumerHarness = harness.GetConsumerHarness<TConsumer>();
            Assert.That(await consumerHarness.Consumed.Any<TEvent>(harness.CancellationToken));

            await harness.ThrowOnConsumeError<TEvent>();
        }

        /// <summary>
        /// Checks if a fault message for <typeparamref name="TMessage"/> was published and throws an exception containing the fault details.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a fault is found, containing details from the first exception in the fault message.</exception>
        public async Task ThrowOnConsumeFault<TMessage>()
            where TMessage : class, IRoutableMessage
        {
            if (!await harness.Published.Any<Fault<TMessage>>(harness.CancellationToken))
                return;

            var consumed = await harness.Published.SelectAsync<Fault<TMessage>>(harness.CancellationToken).ToListAsync();
            var receivedMessage = consumed.FirstOrDefault(k => k.Context.Message.Exceptions.Length > 0);
            if (receivedMessage is null)
                return;

            // there may be more exceptions we just throw the first one
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
        /// if an error occurred it gets swallowed by masstransit - therefore throw it on the tester
        /// </summary>
        /// <exception cref="Exception"></exception>
        private async Task ThrowOnConsumeError<TMessage>()
            where TMessage : class, IRoutableMessage
        {
            var consumed = await harness.Consumed.SelectAsync<TMessage>(harness.CancellationToken).ToListAsync();
            var receivedMessage = consumed.FirstOrDefault(k => k.Exception is not null);
            if (receivedMessage is not null)
                throw receivedMessage.Exception;
        }

        /// <summary>
        /// Executes a routing slip with a single activity and asserts its execution.
        /// </summary>
        public async Task TestExecuteActivity<T, TArguments>(TArguments arguments,
            IEnumerable<KeyValuePair<string, object>>? variables = null)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class, IActivityArgument
        {
            // di harness is shut down automatically
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);
            await harness.Start();
            await Task.Delay(StartDelay, harness.CancellationToken);

            // Act
            var endpointName = harness.EndpointNameFormatter.ExecuteActivity<T, TArguments>();

            // we need to create a routing slip for our activity?!
            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            builder.AddActivity("Testo", new Uri($"queue:{endpointName}"), arguments);

            if (variables is not null)
                builder.SetVariables(variables);

            await harness.Bus.Execute(builder.Build());
            await harness.InactivityTask;
        }
    }

    extension(InMemoryTestHarness harness)
    {
        /// <summary>
        /// Executes an activity and waits for <see cref="RoutingSlipActivityCompleted"/>, returning the context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the tracking number of the completed routing slip does not match.</exception>
        public async Task<ConsumeContext<RoutingSlipActivityCompleted>> TestExecuteActivityCompletedWithVariables<TArguments>(ActivityTestContext activity,
            TArguments arguments,
            IEnumerable<KeyValuePair<string, object>>? variables = null)
            where TArguments : class
        {
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);

            var completed = harness.SubscribeHandler<RoutingSlipCompleted>();
            var activityCompleted = harness.SubscribeHandler<RoutingSlipActivityCompleted>();
            //var activityCompensate = harness.SubscribeHandler<RoutingSlipActivityCompensated>();

            var argumentsDictionary = RoutingSlipBuilder.GetObjectAsDictionary(arguments);

            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(harness.BusAddress, RoutingSlipEvents.All);
            builder.AddActivity(activity.Name, activity.ExecuteUri, arguments);
            builder.AddCompensateLog(trackingNumber, activity.ExecuteUri, argumentsDictionary);

            if (variables is not null)
                builder.SetVariables(variables);

            await harness.Bus.Execute(builder.Build());

            // wait for the routing slip to complete
            await completed;

            // wait for the routing slip activity to complete
            var context = await activityCompleted!;
            await harness.InactivityTask;

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
        /// <exception cref="InvalidOperationException">Thrown if the tracking number of the faulted routing slip does not match.</exception>
        public async Task<ConsumeContext<RoutingSlipActivityFaulted>> TestExecuteActivityFaultedWithVariables<TArguments>(ActivityTestContext activity,
            TArguments arguments)
            where TArguments : class, IActivityArgument
        {
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(InactivityTimeout);
            harness.TestTimeout = TimeSpan.FromSeconds(TestTimeout);

            var faulted = harness.SubscribeHandler<RoutingSlipFaulted>();
            var activityFaulted = harness.SubscribeHandler<RoutingSlipActivityFaulted>();

            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(harness.BusAddress, RoutingSlipEvents.All);
            builder.AddActivity(activity.Name, activity.ExecuteUri, arguments);

            await harness.Bus.Execute(builder.Build());

            // wait for the routing slip to fail
            await faulted;

            // wait for the routing slip activity to fail
            var context = await activityFaulted!;
            await harness.InactivityTask;

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
