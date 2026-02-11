using AwesomeAssertions;
using MassTransit;
using NSubstitute;
using Sdk.Backend.Messaging;
using Sdk.Testing.Backend;
using TestModule.Backend.Consumer;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class ISuiteMediatorExtensionsTests
{
    private readonly ISuiteMediator _suiteMediator = Substitute.For<ISuiteMediator>();

    public class SetupRequest : ISuiteMediatorExtensionsTests
    {
        [Fact]
        public async Task Should_setup_request_response()
        {
            // Arrange
            var request = new TestConsumerRequest(Guid.NewGuid());
            var response = new TestConsumerResponse(request.RequestId);

            // Act
            _suiteMediator.SetupRequest(request, response);

            // Assert
            var result = await _suiteMediator.Request<TestConsumerRequest, TestConsumerResponse>(request, TestContext.Current.CancellationToken);
            result.Should().Be(response);
        }

        [Fact]
        public async Task Should_setup_instance_dependent_request_response()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            var request = new TestInstanceConsumerRequest(Guid.NewGuid());
            var response = new TestInstanceConsumerResponse(request.RequestId);

            // Act
            _suiteMediator.SetupRequest(request, response, instanceId);

            // Assert
            var result = await _suiteMediator.Request<TestInstanceConsumerRequest, TestInstanceConsumerResponse>(request, instanceId, TestContext.Current.CancellationToken);
            result.Should().Be(response);
        }
    }

    public class SetupRequestFault : ISuiteMediatorExtensionsTests
    {
        [Fact]
        public async Task Should_setup_request_response()
        {
            // Arrange
            var request = new TestConsumerRequest(Guid.NewGuid());

            // Act
            _suiteMediator.SetupRequestFault<TestConsumerRequest, TestConsumerResponse>(request);

            // Assert
            var action = () => _suiteMediator.Request<TestConsumerRequest, TestConsumerResponse>(request, TestContext.Current.CancellationToken);
            await action.Should().ThrowAsync<RequestFaultException>();
        }

        [Fact]
        public async Task Should_setup_instance_dependent_request_response()
        {
            // Arrange
            var instanceId = Guid.NewGuid();
            var request = new TestInstanceConsumerRequest(Guid.NewGuid());

            // Act
            _suiteMediator.SetupRequestFault<TestInstanceConsumerRequest, TestInstanceConsumerResponse>(request, instanceId);

            // Assert
            var action = () => _suiteMediator.Request<TestInstanceConsumerRequest, TestInstanceConsumerResponse>(request, instanceId, TestContext.Current.CancellationToken);
            await action.Should().ThrowAsync<RequestFaultException>();
        }
    }
}
