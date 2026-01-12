using System.Text.Json;
using AwesomeAssertions;
using Sdk.Connections;
using Sdk.Connections.Contracts;
using Sdk.Connections.Extensions;
using Sdk.Messaging;
using Sdk.Testing.Factories;

namespace Sdk.Tests.Connections;

public class ConnectionExtensionsTests
{
    private static readonly Guid s_instanceId = Guid.NewGuid();

    private readonly Connection _mqttTlsConnection = ConnectionFactory.CreateMqttServiceConnection("MQTT TLS", protocol: MqttConnectionType.TCPWithTLS);
    private readonly Connection _mqttWebSocketConnection = ConnectionFactory.CreateMqttServiceConnection("MQTT WebSocket", protocol: MqttConnectionType.WebSocket);
    private readonly Connection _dbConnection = new() { Id = Guid.NewGuid(), Type = ConnectionType.Database, Name = "DB" };
    private readonly Connection _httpConnection = new() { Id = Guid.NewGuid(), Type = ConnectionType.Http, Name = "HTTP" };

    public class Assign : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_assign_properties()
        {
            // Arrange
            var connection = new Connection()
            {
                Json = "ConnectionString",
                Description = "Description",
                Name = "Connection",
                Type = ConnectionType.Database,
                Tags = [ConnectionConstants.Tags.SystemDefault],
                Metadata = new Dictionary<string, string?> { { ConnectionConstants.MetaDataKeys.InstanceId, "SomeValue" } }
            };
            var result = new Connection();

            // Act
            result.Assign(connection);

            // Assert
            result.Should().BeEquivalentTo(connection, config => config.Excluding(k => k.Id));
            result.Id.Should().NotBe(connection.Id);
        }
    }

    public sealed class FindInstanceConnections : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_find_existing_instance_connections()
        {
            // Arrange
            var instanceMqtt = new Connection
            {
                Id = Guid.NewGuid(),
                Type = ConnectionType.Mqtt,
                Name = "InstanceMqtt",
                Metadata = new() { { ConnectionConstants.MetaDataKeys.InstanceId, s_instanceId.ToString() } },
            };

            var instanceDb = new Connection
            {
                Id = Guid.NewGuid(),
                Type = ConnectionType.Database,
                Name = "InstanceDb",
                Metadata = new() { { ConnectionConstants.MetaDataKeys.InstanceId, s_instanceId.ToString() } },
            };

            var connections = new List<Connection>
            {
                _mqttTlsConnection,
                instanceMqtt,
                _dbConnection,
                instanceDb,
            };

            // Act
            var result = connections.FindInstanceConnections(s_instanceId).ToArray();

            // Assert
            result.Should().ContainSingle(k => k.Id == instanceMqtt.Id);
            result.Should().ContainSingle(k => k.Id == instanceDb.Id);
        }

        [Fact]
        public void Should_return_no_items_if_no_instance_connections_exist()
        {
            // Arrange
            var connections = new List<Connection>
            {
                _mqttTlsConnection,
                _httpConnection,
                _dbConnection,
                _mqttWebSocketConnection,
            };

            // Act
            var result = connections.FindInstanceConnections(s_instanceId).ToArray();

            // Assert
            result.Should().BeEmpty();
        }
    }

    public sealed class FirstOrDefaultInstanceMqttConnection : ConnectionExtensionsTests
    {
        private readonly Connection _tcpMqttConnection1 = new()
        {
            Id = Guid.NewGuid(),
            Type = ConnectionType.Mqtt,
            Name = "InstanceMqtt1",
            Metadata = new()
            {
                { ConnectionConstants.MetaDataKeys.InstanceId, s_instanceId.ToString() },
                { ConnectionConstants.MetaDataKeys.MqttClientProtocol, MqttConnectionType.TCP.ToString() },
            },
        };

        private readonly Connection _tcpMqttConnection2 = new()
        {
            Id = Guid.NewGuid(),
            Type = ConnectionType.Mqtt,
            Name = "InstanceMqtt2",
            Metadata = new()
            {
                { ConnectionConstants.MetaDataKeys.InstanceId, s_instanceId.ToString() },
                { ConnectionConstants.MetaDataKeys.MqttClientProtocol, MqttConnectionType.TCP.ToString() },
            },
        };

        private readonly Connection _websocketMqttConnection = new()
        {
            Id = Guid.NewGuid(),
            Type = ConnectionType.Database,
            Name = "InstanceMqtt2",
            Metadata = new()
            {
                { ConnectionConstants.MetaDataKeys.InstanceId, s_instanceId.ToString() },
                { ConnectionConstants.MetaDataKeys.MqttClientProtocol, MqttConnectionType.WebSocket.ToString() },
            },
        };

        [Fact]
        public void Should_find_first_available_instance_connection()
        {
            // Arrange
            var connections = new List<Connection>
            {
                _mqttTlsConnection,
                _tcpMqttConnection1,
                _dbConnection,
                _tcpMqttConnection2,
                _websocketMqttConnection,
            };

            // Act
            var result = connections.FirstOrDefaultInstanceMqttConnection(s_instanceId, MqttConnectionType.TCP);

            // Assert
            result.Should().BeEquivalentTo(_tcpMqttConnection1);
        }

        [Fact]
        public void Should_return_null_if_no_instance_mqtt_connection_exists()
        {
            // Arrange
            var connections = new List<Connection>
            {
                _mqttTlsConnection,
                _dbConnection,
                _websocketMqttConnection,
            };

            // Act
            var result = connections.FirstOrDefaultInstanceMqttConnection(s_instanceId, MqttConnectionType.TCP);

            // Assert
            result.Should().BeNull();
        }
    }

    public sealed class GetAzureIotHubConnection : ConnectionExtensionsTests
    {
        private readonly AzureIotHubConnection _connection = new()
        {
            Hostname = "localhost",
            SharedAccessSignatureKey = "xxx",
            SharedAccessSignatureKeyName = "yyy",
        };

        [Fact]
        public void Should_get_inner_connection_for_matching_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Azure" };
            connection.SetAzureIotHubConnection(_connection);

            // Act
            var result = connection.GetAzureIotHubConnection();

            // Assert
            result.Should().BeEquivalentTo(_connection);
        }

        [Fact]
        public void Should_return_null_on_connection_type_mismatch()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Azure" };
            connection.SetAzureIotHubConnection(_connection);
            connection.Type = ConnectionType.Http;

            // Act
            var result = connection.GetAzureIotHubConnection();

            // Assert
            result.Should().BeNull();
        }
    }

    public sealed class GetDatabaseConnection : ConnectionExtensionsTests
    {
        private readonly DatabaseConnection _connection = new()
        {
            ConnectionString = "DataSource=XXX;Initial Catalog=DB;User ID=user;Password=password;",
            DatabaseType = DatabaseConnectionType.XPO,
        };

        [Fact]
        public void Should_get_inner_connection_for_matching_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Db" };
            connection.SetDatabaseConnection(_connection);

            // Act
            var result = connection.GetDatabaseConnection();

            // Assert
            result.Should().BeEquivalentTo(_connection);
        }

        [Fact]
        public void Should_return_null_on_connection_type_mismatch()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Db" };
            connection.SetDatabaseConnection(_connection);
            connection.Type = ConnectionType.Http;

            // Act
            var result = connection.GetDatabaseConnection();

            // Assert
            result.Should().BeNull();
        }
    }

    public sealed class GetHttpConnection : ConnectionExtensionsTests
    {
        private readonly HttpConnection _connection = new()
        {
            BaseAddress = "localhost",
            ApiKey = "XDFGASDFERC123",
        };

        [Fact]
        public void Should_get_inner_connection_for_matching_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Http" };
            connection.SetHttpConnection(_connection);

            // Act
            var result = connection.GetHttpConnection();

            // Assert
            result.Should().BeEquivalentTo(_connection);
        }

        [Fact]
        public void Should_return_null_on_connection_type_mismatch()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Http" };
            connection.SetHttpConnection(_connection);
            connection.Type = ConnectionType.Database;

            // Act
            var result = connection.GetHttpConnection();

            // Assert
            result.Should().BeNull();
        }
    }

    public sealed class GetMqttConnection : ConnectionExtensionsTests
    {
        private readonly MqttConnection _mqttConnection = new()
        {
            Address = "localhost",
            Username = "user",
            Password = "password",
            Protocol = MqttConnectionType.TCP,
            ClientId = "xyz",
            Port = 1883,
            WillTopic = "topic",
            WillRetain = true,
            WillMessage = "message",
        };

        [Fact]
        public void Should_get_inner_connection_for_matching_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Mqtt" };
            connection.SetMqttConnection(_mqttConnection);

            // Act
            var result = connection.GetMqttConnection();

            // Assert
            result.Should().BeEquivalentTo(_mqttConnection);
        }

        [Fact]
        public void Should_return_null_on_connection_type_mismatch()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Mqtt" };
            connection.SetMqttConnection(_mqttConnection);
            connection.Type = ConnectionType.Http;

            // Act
            var result = connection.GetMqttConnection();

            // Assert
            result.Should().BeNull();
        }
    }

    public class GetWebsocketUri : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_build_websocket_uri()
        {
            // Arrange
            var mqtt = new MqttConnection
            {
                Address = "127.0.0.1/mqtt",
                Port = 1883,
                Protocol = MqttConnectionType.TCP
            };

            // Act
            var url = mqtt.GetWebsocketUri();

            // Assert
            url.AbsoluteUri.Should().Be("ws://127.0.0.1:1883/mqtt");
        }
    }

    public sealed class SetAzureIotHubConnection : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_set_json_and_connection_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Azure" };
            var azureConnection = new AzureIotHubConnection
            {
                Hostname = "localhost",
                SharedAccessSignatureKey = "xxx",
                SharedAccessSignatureKeyName = "yyy",
            };

            // Act
            connection.SetAzureIotHubConnection(azureConnection);

            // Assert
            AssertInnerConnection(connection, ConnectionType.AzureIotHub, azureConnection);
        }
    }

    public sealed class SetDatabaseConnection : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_set_json_and_connection_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Db" };
            var dbConnection = new DatabaseConnection
            {
                ConnectionString = "DataSource=XXX;Initial Catalog=DB;User ID=user;Password=password;",
                DatabaseType = DatabaseConnectionType.XPO,
            };

            // Act
            connection.SetDatabaseConnection(dbConnection);

            // Assert
            AssertInnerConnection(connection, ConnectionType.Database, dbConnection);
        }
    }

    public sealed class SetHttpConnection : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_set_json_and_connection_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Http" };
            var httpConnection = new HttpConnection
            {
                BaseAddress = "localhost",
                ApiKey = "XDFGASDFERC123",
            };

            // Act
            connection.SetHttpConnection(httpConnection);

            // Assert
            AssertInnerConnection(connection, ConnectionType.Http, httpConnection);
        }
    }

    public sealed class SetMqttConnection : ConnectionExtensionsTests
    {
        [Fact]
        public void Should_set_json_and_connection_type()
        {
            // Arrange
            var connection = new Connection { Id = Guid.NewGuid(), Name = "Mqtt" };
            var mqttConnection = new MqttConnection
            {
                Address = "localhost",
                Username = "user",
                Password = "password",
                Protocol = MqttConnectionType.TCP,
                ClientId = "xyz",
                Port = 1883,
                WillTopic = "topic",
                WillRetain = true,
                WillMessage = "message",
            };

            // Act
            connection.SetMqttConnection(mqttConnection);

            // Assert
            AssertInnerConnection(connection, ConnectionType.Mqtt, mqttConnection);
        }
    }

    private static void AssertInnerConnection<TInnerConnection>(Connection connection, ConnectionType expectedType, TInnerConnection expectedInnerConnection)
    {
        connection.Type.Should().Be(expectedType);
        Assert.NotNull(connection.Json);

        JsonSerializer.Deserialize<TInnerConnection>(connection.Json, DefaultJsonSerializerSettings.Default)
            .Should().BeEquivalentTo(expectedInnerConnection);
    }
}
