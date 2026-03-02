using AwesomeAssertions;
using Sdk.Connections.Contracts;
using Sdk.Testing.Factories;
using Xunit;

namespace Sdk.Testing.Tests.Factories;

public sealed class ConnectionFactoryTests
{
    [Fact]
    public void CreateMqttServiceConnection_should_create_connection_with_defaults()
    {
        // Act
        var connection = ConnectionFactory.CreateMqttServiceConnection("test-connection");

        // Assert
        connection.Should().NotBeNull();
        connection.Name.Should().Be("test-connection");
        connection.Type.Should().Be(ConnectionType.Mqtt);
        connection.Id.Should().NotBeEmpty();
        connection.Tags.Should().BeEmpty();
    }

    [Fact]
    public void CreateMqttServiceConnection_should_set_custom_address_and_port()
    {
        // Act
        var connection = ConnectionFactory.CreateMqttServiceConnection("mqtt-conn", address: "broker.example.com", port: 8883);

        // Assert
        connection.Name.Should().Be("mqtt-conn");
        connection.Json.Should().NotBeNullOrEmpty();
        connection.Json.Should().Contain("broker.example.com");
    }

    [Fact]
    public void CreateMqttServiceConnection_should_include_tags_when_provided()
    {
        // Arrange
        var tags = new[]
        {
            new Tag("tag1"),
            new Tag("tag2")
        };

        // Act
        var connection = ConnectionFactory.CreateMqttServiceConnection("tagged-conn", tags: tags);

        // Assert
        connection.Tags.Should().HaveCount(2);
    }

    [Fact]
    public void CreateMqttServiceConnection_should_set_protocol()
    {
        // Act
        var connection = ConnectionFactory.CreateMqttServiceConnection("ws-conn", protocol: MqttConnectionType.WebSocket);

        // Assert
        connection.Json.Should().NotBeNullOrEmpty();
    }
}

