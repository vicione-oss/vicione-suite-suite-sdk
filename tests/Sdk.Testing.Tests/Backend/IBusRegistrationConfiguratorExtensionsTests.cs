using AwesomeAssertions;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Messaging;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class IBusRegistrationConfiguratorExtensionsTests
{
    [Fact]
    public void Should_setup_routing_slip_builder_factory()
    {
        // Arrange
        var configurator = new ServiceCollectionBusConfigurator(new ServiceCollection());

        // Act
        var serviceProvider = configurator
            .AddRoutingSlipBuilderFactory()
            .BuildServiceProvider();

        // Assert

        serviceProvider.GetService<IRoutingSlipBuilderFactory>().Should().NotBeNull();
    }

    [Fact]
    public void Should_setup_routing_slip_builder()
    {
        // Arrange
        var configurator = new ServiceCollectionBusConfigurator(new ServiceCollection());
        var trackingNumber = Guid.NewGuid();

        // Act
        var serviceProvider = configurator
            .AddRoutingSlipBuilderFactory()
            .BuildServiceProvider();

        // Assert
        var factory = serviceProvider.GetRequiredService<IRoutingSlipBuilderFactory>();
        var builder = factory.Create(trackingNumber);
        builder.TrackingNumber.Should().Be(trackingNumber);
    }
}
