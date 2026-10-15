using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using Sdk.SystemConfiguration.Contracts;

namespace Sdk.Tests.SystemConfiguration;

public sealed class IPAddressListConverterTests
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        Converters = { new IPAddressListConverter() },
    };

    [Fact]
    public void Addresses_roundtrip_through_json_in_order()
    {
        // Arrange
        IReadOnlyList<IPAddress> addresses =
        [
            IPAddress.Parse("8.8.8.8"),
            IPAddress.Parse("2001:4860:4860::8888"),
            IPAddress.Loopback,
        ];

        // Act
        var json = JsonSerializer.Serialize(addresses, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<IReadOnlyList<IPAddress>>(json, s_jsonOptions);

        // Assert
        json.Should().Be("[\"8.8.8.8\",\"2001:4860:4860::8888\",\"127.0.0.1\"]");
        deserialized.Should().Equal(addresses);
    }

    [Fact]
    public void Empty_array_roundtrips_through_json()
    {
        // Arrange
        IReadOnlyList<IPAddress> addresses = [];

        // Act
        var json = JsonSerializer.Serialize(addresses, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<IReadOnlyList<IPAddress>>(json, s_jsonOptions);

        // Assert
        json.Should().Be("[]");
        deserialized.Should().BeEmpty();
    }

    [Theory]
    [InlineData("\"8.8.8.8\"")]
    [InlineData("{}")]
    [InlineData("5")]
    [InlineData("[5]")]
    [InlineData("[null]")]
    [InlineData("[\"\"]")]
    [InlineData("[\"not-an-address\"]")]
    [InlineData("[\"8.8.8.8\",\"not-an-address\"]")]
    [InlineData("[[\"8.8.8.8\"]]")]
    public void Reading_a_non_array_or_an_unparsable_element_throws(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<IReadOnlyList<IPAddress>>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }
}
