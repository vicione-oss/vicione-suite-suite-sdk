using System.Net.Http.Json;
using AwesomeAssertions;
using Sdk.Connections.Contracts;
using Sdk.Testing.Client;
using Sdk.Testing.Factories;
using Xunit;

namespace Sdk.Testing.Tests.Client;

public class HttpClientFactoryTests
{
    [Fact]
    public async Task Should_create_http_client_that_handles_request_successfull()
    {
        // Arrange
        var response = ConnectionFactory.CreateMqttServiceConnection("test");

        // Act
        using var httpClient = HttpClientFactory.GetHttpClientWithResponse(response);

        // Assert
        var clientResponse = await httpClient.GetAsync(new Uri("/some/uri", UriKind.Relative));
        clientResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Should_create_http_client_with_fixed_response()
    {
        // Arrange
        var response = ConnectionFactory.CreateMqttServiceConnection("test");

        // Act
        using var httpClient = HttpClientFactory.GetHttpClientWithResponse(response);

        // Assert
        var connection = await httpClient.GetFromJsonAsync<Connection>(new Uri("/some/uri", UriKind.Relative));
        connection.Should().BeEquivalentTo(response);
    }
}
