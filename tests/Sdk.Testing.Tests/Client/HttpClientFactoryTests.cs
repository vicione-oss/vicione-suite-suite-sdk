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
        var clientResponse = await httpClient.GetAsync(new Uri("/some/uri", UriKind.Relative), CancellationToken.None);
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
        var connection = await httpClient.GetFromJsonAsync<Connection>(new Uri("/some/uri", UriKind.Relative), CancellationToken.None);
        connection.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Should_answer_every_request_with_the_full_response()
    {
        // Arrange
        var response = ConnectionFactory.CreateMqttServiceConnection("test");
        using var httpClient = HttpClientFactory.GetHttpClientWithResponse(response);
        var uri = new Uri("/some/uri", UriKind.Relative);

        // Act
        var first = await httpClient.GetFromJsonAsync<Connection>(uri, CancellationToken.None);
        using var second = await httpClient.GetAsync(uri, CancellationToken.None);
        var secondConnection = await second.Content.ReadFromJsonAsync<Connection>(CancellationToken.None);

        // Assert
        first.Should().BeEquivalentTo(response);
        secondConnection.Should().BeEquivalentTo(response);
        second.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }
}
