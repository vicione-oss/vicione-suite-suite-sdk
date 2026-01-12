using AwesomeAssertions;
using Sdk.Client.Infrastructure;
using Sdk.Connections.Contracts;
using Sdk.Connections.Requests;
using Sdk.Testing.Client;
using Sdk.Testing.Factories;
using Xunit;

namespace Sdk.Testing.Tests.Client;

public class IUiMediatorExtensionsTests
{
    public class SetupGetConnections
    {
        [Fact]
        public async Task Should_return_parameter_connections_on_request()
        {
            // Arrange
            var mediator = NSubstitute.Substitute.For<IUiMediator>();
            var request = new GetConnections(null, null);
            var connections = new List<Connection>
            {
                ConnectionFactory.CreateMqttServiceConnection("mqtt1"),
                ConnectionFactory.CreateMqttServiceConnection("mqtt2"),
                ConnectionFactory.CreateMqttServiceConnection("mqtt3"),
            };

            // Act
            mediator.SetupGetConnections(connections);

            // Assert
            var response = await mediator.Request<GetConnections, GetConnectionsResponse>(request);
            response.Should().Be(response);

        }

        [Fact]
        public async Task Should_return_empty_response_without_parameters()
        {
            // Arrange
            var mediator = NSubstitute.Substitute.For<IUiMediator>();
            var request = new GetConnections(null, null);

            // Act
            mediator.SetupGetConnections();

            // Assert
            var response = await mediator.Request<GetConnections, GetConnectionsResponse>(request);
            response.Connections.Should().BeEmpty();
        }
    }

    public class SetupGetSingleConnection
    {
        [Fact]
        public async Task Should_return_parameter_connection_on_request()
        {
            // Arrange
            var mediator = NSubstitute.Substitute.For<IUiMediator>();
            var request = new GetConnections(null, null);
            var connection = ConnectionFactory.CreateMqttServiceConnection("mqtt1");

            // Act
            mediator.SetupGetSingleConnection(connection);

            // Assert
            var response = await mediator.Request<GetConnections, GetConnectionsResponse>(request);
            response.Connections.Should().Contain(connection);
        }
    }
}
