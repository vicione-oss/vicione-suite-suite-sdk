using AwesomeAssertions;
using Sdk.Connections;
using Sdk.Connections.Contracts;

namespace Sdk.Tests.Connections;

public sealed class DefaultJsonConnectionSerializerTests
{
    public sealed class Serialize
    {
        [Fact]
        public void Should_serialize_http_connection()
        {
            // Arrange
            var serializer = new DefaultJsonConnectionSerializer<HttpConnection>();
            var connection = new HttpConnection { BaseAddress = "https://example.com", ApiKey = "key123" };

            // Act
            var json = serializer.Serialize(connection);

            // Assert
            json.Should().Contain("https://example.com");
            json.Should().Contain("key123");
        }

        [Fact]
        public void Should_serialize_postgres_connection()
        {
            // Arrange
            var serializer = new DefaultJsonConnectionSerializer<PostgresConnection>();
            var connection = new PostgresConnection { ConnectionString = "Host=localhost;Database=test" };

            // Act
            var json = serializer.Serialize(connection);

            // Assert
            json.Should().Contain("Host=localhost;Database=test");
        }
    }

    public sealed class Deserialize
    {
        [Fact]
        public void Should_deserialize_http_connection()
        {
            // Arrange
            var serializer = new DefaultJsonConnectionSerializer<HttpConnection>();
            const string Json = """{"BaseAddress":"https://example.com","ApiKey":"key123"}""";

            // Act
            var connection = serializer.Deserialize(Json);

            // Assert
            connection.Should().NotBeNull();
            connection.Should().BeOfType<HttpConnection>();
            var httpConnection = (HttpConnection)connection!;
            httpConnection.BaseAddress.Should().Be("https://example.com");
            httpConnection.ApiKey.Should().Be("key123");
        }

        [Fact]
        public void Should_roundtrip_connection()
        {
            // Arrange
            var serializer = new DefaultJsonConnectionSerializer<HttpConnection>();
            var original = new HttpConnection { BaseAddress = "https://api.test.com", ApiKey = "secret" };

            // Act
            var json = serializer.Serialize(original);
            var deserialized = serializer.Deserialize(json) as HttpConnection;

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.BaseAddress.Should().Be(original.BaseAddress);
            deserialized.ApiKey.Should().Be(original.ApiKey);
        }
    }
}

