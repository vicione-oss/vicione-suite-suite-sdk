using MassTransit;
using MassTransit.Courier.Contracts;
using Sdk.Backend.Messaging;
using Sdk.Messaging;
using Xunit;

namespace Sdk.Backend.Tests.Messaging;

public sealed class MessagingHelperTests
{
    private const string ActivityEndpoint = "RegisterInstanceActivity";
    private const string CommandEndpoint = "RegisterInstanceCommand";

    public sealed class GetRequestEndpointAddress
    {
        [Fact]
        public void Should_return_correct_endpoint_address_for_instance_independent()
        {
            // Act
            var endpointUri = MessagingHelper.GetRequestEndpointAddress<InstanceIndependentRequest, Response>();

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"exchange:{MessagingHelper.RequestsQueueName}", endpointUri.ToString());
        }

        [Fact]
        public void Should_return_correct_endpoint_address_for_instance_dependent()
        {
            // Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var endpointUri = MessagingHelper.GetRequestEndpointAddress<InstanceDependentRequest, Response>(instanceId);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"exchange:{MessagingHelper.InstanceQueueNamePrefix}_{instanceId}", endpointUri.ToString());
        }
    }

    public sealed class GetCommandEndpointAddress
    {
        [Fact]
        public void Should_return_correct_endpoint_address_for_default_endpoint()
        {
            // Act
            var endpointUri = MessagingHelper.GetCommandEndpointAddress(typeof(SomeCommand), null);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{MessagingHelper.CommandsQueueName}", endpointUri.ToString());
        }

        [Fact]
        public void Should_return_correct_endpoint_address_for_own_endpoint()
        {
            // Act
            var endpointUri = MessagingHelper.GetCommandEndpointAddress<OwnEndpointCommand>();

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{CommandEndpoint}", endpointUri.ToString());
        }

        [Fact]
        public void Should_return_correct_endpoint_address_for_instance_dependent_generic()
        {
            // Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var endpointUri = MessagingHelper.GetCommandEndpointAddress<DefaultEndpointInstanceDependentCommand>(instanceId);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{MessagingHelper.InstanceQueueNamePrefix}_{instanceId}", endpointUri.ToString());
        }

        [Theory]
        [InlineData(typeof(DefaultEndpointInstanceDependentCommand))]
        [InlineData(typeof(OwnEndpointInstanceDependentCommand))]
        public void Should_return_correct_endpoint_address_for_instance_dependent(Type messageType)
        {
            // Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var endpointUri = MessagingHelper.GetCommandEndpointAddress(messageType, instanceId);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{MessagingHelper.InstanceQueueNamePrefix}_{instanceId}", endpointUri.ToString());
        }
    }

    public sealed class GetActivityEndpointAddress
    {
        [Fact]
        public void Should_return_correct_endpoint_address_for_instance_dependent()
        {
            //Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var endpointUri = MessagingHelper.GetActivityEndpointAddress<DataActivityOwnEndpointInstanceDependent>(instanceId);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{ActivityEndpoint}_{instanceId}", endpointUri.ToString());
        }

        [Fact]
        public void Should_return_correct_endpoint_address_for_instance_independent()
        {
            // Act
            var endpointUri = MessagingHelper.GetActivityEndpointAddress<DataActivityOwnEndpointInstanceIndependent>(null);

            // Assert
            Assert.NotNull(endpointUri);
            Assert.Equal($"{MessagingHelper.ExchangePrefix}:{ActivityEndpoint}", endpointUri.ToString());
        }

        [Fact]
        public void Should_throw_if_MessageEndpointAttribute_is_missing()
            => Assert.Throws<InvalidOperationException>(() => MessagingHelper.GetActivityEndpointAddress<InvalidActivity>(null));
    }

    public sealed class ConsumesInstanceDependentMessages
    {
        [Fact]
        public void Should_return_is_valid_or_invalid()
            => Assert.True(typeof(ValidInstanceDependentEventConsumer).ConsumesInstanceDependentMessages());
    }

    public sealed class ConsumesRequest
    {
        [Theory]
        [InlineData(typeof(InstanceIndependentRequestConsumer))]
        [InlineData(typeof(DerivedRequestConsumer))]
        public void Should_return_true_for_a_request_consumer(Type consumerType)
            => Assert.True(consumerType.ConsumesRequest());

        [Theory]
        [InlineData(typeof(EventFaultConsumer))]
        [InlineData(typeof(ValidInstanceDependentEventConsumerDefinition))]
        [InlineData(typeof(MessagelessConsumer))]
        public void Should_return_false_for_a_consumer_without_request_message_types(Type consumerType)
            => Assert.False(consumerType.ConsumesRequest());

        [Fact]
        public void Should_return_false_for_a_consumer_that_also_handles_something_else()
            => Assert.False(typeof(RequestAndFaultConsumer).ConsumesRequest());

        [Fact]
        public void Should_return_false_for_a_consumer_of_events_only()
            => Assert.False(typeof(ValidInstanceDependentEventConsumer).ConsumesRequest());
    }

    public sealed class FindAllMessageTypes
    {
        [Fact]
        public void Should_return_generic_message_types_that_FindMessageTypes_filters_out()
        {
            // Act
            var types = typeof(ValidInstanceDependentEventConsumerWithFaultHandler).FindAllMessageTypes().ToArray();

            // Assert
            Assert.Equal(2, types.Length);
            Assert.Contains(typeof(InstanceDependentEvent), types);
            Assert.Contains(typeof(Fault<InstanceDependentEvent>), types);
        }

        [Fact]
        public void Should_return_the_routing_slip_contracts()
        {
            // Act
            var types = typeof(ValidInstanceDependentEventConsumerWithRoutingSlip).FindAllMessageTypes().ToArray();

            // Assert
            Assert.Equal(3, types.Length);
            Assert.Contains(typeof(RoutingSlipCompleted), types);
            Assert.Contains(typeof(RoutingSlipFaulted), types);
        }

        [Fact]
        public void Should_return_nothing_for_a_type_that_consumes_no_message()
        {
            // Act
            var types = typeof(ValidInstanceDependentEventConsumerDefinition).FindAllMessageTypes().ToArray();

            // Assert
            Assert.Empty(types);
        }
    }

    public sealed class IsInstanceDependent
    {
        [Theory]
        [InlineData(typeof(InstanceDependentEvent))]
        [InlineData(typeof(InstanceDependentRequest))]
        public void Should_return_true_for_instance_dependent(Type type)
            => Assert.True(type.IsInstanceDependent());

        [Theory]
        [InlineData(typeof(InstanceIndependentEvent))]
        [InlineData(typeof(ConsumerActivityOwnEndpointInstanceIndependent))]
        public void Should_return_false_for_instance_independent(Type type)
            => Assert.False(type.IsInstanceDependent());
    }

    public sealed class GetEndpointName
    {
        [Fact]
        public void Should_return_EndpointName()
            => Assert.Equal(CommandEndpoint, typeof(OwnEndpointCommand).GetEndpointName());

        [Fact]
        public void Should_return_nothing_if_not_attribute()
            => Assert.Null(typeof(SomeCommand).GetEndpointName());
    }

    public sealed class FindMessageTypes
    {
        [Theory]
        [InlineData(typeof(ValidInstanceDependentEventConsumer), typeof(InstanceDependentEvent))]
        [InlineData(typeof(ValidInstanceDependentEventConsumerWithFaultHandler), typeof(InstanceDependentEvent))]
        [InlineData(typeof(ValidInstanceDependentEventConsumerWithRoutingSlip), typeof(InstanceDependentEvent))]
        [InlineData(typeof(ConsumerActivityOwnEndpointInstanceIndependent), typeof(DataActivityOwnEndpointInstanceIndependent))]
        public void Should_return_correct_MessageType(Type consumerType, Type messageType)
        {
            // Act
            var types = consumerType.FindMessageTypes().ToArray();

            // Assert
            Assert.Single(types);
            Assert.Contains(messageType, types);
        }

        [Fact]
        public void Should_return_MessageTypes_of_InvalidMultiMessageConsumer()
        {
            // Act
            var types = typeof(InvalidMultiMessageConsumer).FindMessageTypes().ToArray();

            // Assert
            Assert.Equal(2, types.Length);
            Assert.Contains(typeof(SomeCommand), types);
            Assert.Contains(typeof(InstanceDependentEvent), types);
        }
    }

    public sealed class GetActivityEndpointName
    {
        [Fact]
        public void Should_return_correct_endpoint_for_instance_dependent()
        {
            // Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var name = typeof(DataActivityOwnEndpointInstanceIndependent).GetActivityEndpointName(instanceId);

            // Assert
            Assert.Equal(ActivityEndpoint, name);
        }

        [Fact]
        public void Should_return_correct_endpoint_for_instance_independent()
        {
            // Arrange
            var instanceId = Guid.NewGuid();

            // Act
            var name = typeof(DataActivityOwnEndpointInstanceDependent).GetActivityEndpointName(instanceId);

            // Assert
            Assert.Equal($"{ActivityEndpoint}_{instanceId}", name);
        }

        [Fact]
        public void Should_throw_if_MessageEndpointAttribute_is_missing()
             => Assert.Throws<InvalidOperationException>(() => typeof(InvalidActivity).GetActivityEndpointName(Guid.NewGuid()));

        [Fact]
        public void Should_throw_if_instance_dependent_but_no_instance_id()
             => Assert.Throws<InvalidOperationException>(() => typeof(DataActivityOwnEndpointInstanceDependent).GetActivityEndpointName(null));
    }

    private sealed record InstanceDependentEvent : IInstanceEvent;

    private sealed record InstanceIndependentEvent : IEvent;

    private sealed record SomeCommand(Guid CorrelationId) : ICommand;

    [MessageEndpoint(CommandEndpoint)]
    private sealed record OwnEndpointCommand(Guid CorrelationId) : ICommand;

    private sealed record DefaultEndpointInstanceDependentCommand(Guid CorrelationId) : IInstanceDependentCommand;

    [MessageEndpoint(CommandEndpoint)]
    private sealed record OwnEndpointInstanceDependentCommand(Guid CorrelationId) : IInstanceDependentCommand;

    private sealed record InstanceIndependentRequest : IRequest<Response>;

    private sealed record InstanceDependentRequest : IInstanceDependentRequest<Response>;

    private sealed record Response(ErrorInfo? RequestError) : IResponse;

    [MessageEndpoint(ActivityEndpoint)]
    private sealed record DataActivityOwnEndpointInstanceIndependent : IActivityArgument;

    [MessageEndpoint(ActivityEndpoint)]
    private sealed record DataActivityOwnEndpointInstanceDependent : IInstanceDependentActivityArgument;

    private sealed record InvalidActivity : IActivityArgument;

    private sealed class ValidInstanceDependentEventConsumer : IConsumer<InstanceDependentEvent>
    {
        public Task Consume(ConsumeContext<InstanceDependentEvent> context)
            => Task.CompletedTask;
    }

    private sealed class ValidInstanceDependentEventConsumerWithFaultHandler
        : IConsumer<InstanceDependentEvent>, IConsumer<Fault<InstanceDependentEvent>>
    {
        public Task Consume(ConsumeContext<InstanceDependentEvent> context)
            => Task.CompletedTask;

        public Task Consume(ConsumeContext<Fault<InstanceDependentEvent>> context)
            => Task.CompletedTask;
    }

    private sealed class ValidInstanceDependentEventConsumerWithRoutingSlip
        : IConsumer<InstanceDependentEvent>, IConsumer<RoutingSlipCompleted>, IConsumer<RoutingSlipFaulted>
    {
        public Task Consume(ConsumeContext<InstanceDependentEvent> context)
            => Task.CompletedTask;

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
            => Task.CompletedTask;

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
            => Task.CompletedTask;
    }

    private sealed class ConsumerActivityOwnEndpointInstanceIndependent : IConsumer<DataActivityOwnEndpointInstanceIndependent>
    {
        public Task Consume(ConsumeContext<DataActivityOwnEndpointInstanceIndependent> context)
            => Task.CompletedTask;
    }

    private sealed class InvalidMultiMessageConsumer : IConsumer<SomeCommand>, IConsumer<InstanceDependentEvent>
    {
        public Task Consume(ConsumeContext<InstanceDependentEvent> context)
            => Task.CompletedTask;

        public Task Consume(ConsumeContext<SomeCommand> context)
            => Task.CompletedTask;
    }

    private sealed class ValidInstanceDependentEventConsumerDefinition : ConsumerDefinition<ValidInstanceDependentEventConsumer>;

    private sealed class EventFaultConsumer : IConsumer<Fault<InstanceIndependentEvent>>
    {
        public Task Consume(ConsumeContext<Fault<InstanceIndependentEvent>> context)
            => Task.CompletedTask;
    }

    private sealed class MessagelessConsumer : IConsumer;

    private sealed class InstanceIndependentRequestConsumer : IConsumer<InstanceIndependentRequest>
    {
        public Task Consume(ConsumeContext<InstanceIndependentRequest> context)
            => Task.CompletedTask;
    }

    private sealed class RequestAndFaultConsumer : IConsumer<InstanceIndependentRequest>, IConsumer<Fault<InstanceIndependentRequest>>
    {
        public Task Consume(ConsumeContext<InstanceIndependentRequest> context)
            => Task.CompletedTask;

        public Task Consume(ConsumeContext<Fault<InstanceIndependentRequest>> context)
            => Task.CompletedTask;
    }

    private sealed class DerivedRequestConsumer : RequestConsumer<InstanceIndependentRequest, Response>
    {
        public override Task<Response> Respond(InstanceIndependentRequest message, CancellationToken cancellationToken)
            => Task.FromResult(new Response(null));

        public override Task<Response> HandleException(InstanceIndependentRequest message, Exception e, CancellationToken cancellationToken)
            => Task.FromResult(new Response(new ErrorInfo(0, e.Message)));
    }
}
