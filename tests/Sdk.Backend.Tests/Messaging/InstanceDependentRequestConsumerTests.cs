using AwesomeAssertions;
using MassTransit;
using Sdk.Testing.Backend;
using TestModule.Backend.Consumer;
using Xunit;

namespace Sdk.Backend.Tests.Messaging;

public sealed class InstanceDependentRequestConsumerTests
{
    private readonly Action<IBusRegistrationConfigurator> _configureServices =
        cfg => cfg.AddConsumer<TestInstanceRequestConsumer>();

    [Fact]
    public async Task Consume_should_respond_with_successful_result()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var requestId = Guid.NewGuid();
        var request = new TestInstanceConsumerRequest(requestId);

        // Act
        var response = await tester.TestInstanceDependentRequest<TestInstanceConsumerResponse, TestInstanceConsumerRequest>(request);

        // Assert
        response.RequestId.Should().Be(requestId);
        response.RequestError.Should().BeNull();
    }

    [Fact]
    public async Task Consume_should_respond_with_error_when_exception_occurs()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var requestId = Guid.NewGuid();
        var request = new TestInstanceConsumerRequest(requestId) { ThrowException = true };

        // Act
        var response = await tester.TestInstanceDependentRequest<TestInstanceConsumerResponse, TestInstanceConsumerRequest>(request);

        // Assert
        response.RequestId.Should().Be(requestId);
        response.RequestError.Should().NotBeNull();
        response.RequestError!.ErrorCode.Should().Be(100);
    }
}
