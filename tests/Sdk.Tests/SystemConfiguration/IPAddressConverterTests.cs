using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using Sdk.SystemConfiguration.Contracts;

namespace Sdk.Tests.SystemConfiguration;

public sealed class IPAddressConverterTests
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        Converters = { new IPAddressConverter() },
    };

    [Theory]
    [InlineData(typeof(IPAddress), true)]
    [InlineData(typeof(object), false)]
    [InlineData(typeof(IFormattable), false)]
    [InlineData(typeof(string), false)]
    public void CanConvert_accepts_only_ip_addresses(Type typeToConvert, bool expected)
    {
        // Arrange
        var converter = new IPAddressConverter();

        // Act
        var result = converter.CanConvert(typeToConvert);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CanConvert_accepts_the_runtime_type_of_a_well_known_address()
    {
        // Arrange
        var converter = new IPAddressConverter();
        var runtimeType = IPAddress.Loopback.GetType();
        runtimeType.Should().NotBe<IPAddress>("the case is about a subclass of IPAddress");

        // Act
        var result = converter.CanConvert(runtimeType);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("192.168.1.100")]
    [InlineData("2001:4860:4860::8888")]
    public void Address_roundtrips_through_json(string address)
    {
        // Arrange
        var ipAddress = IPAddress.Parse(address);

        // Act
        var json = JsonSerializer.Serialize(ipAddress, s_jsonOptions);
        var deserialized = JsonSerializer.Deserialize<IPAddress>(json, s_jsonOptions);

        // Assert
        json.Should().Be($"\"{address}\"");
        deserialized.Should().Be(ipAddress);
    }

    [Fact]
    public void Well_known_address_in_an_object_slot_is_written_as_string()
    {
        // Arrange
        object value = IPAddress.Loopback;

        // Act
        var json = JsonSerializer.Serialize(value, s_jsonOptions);

        // Assert
        json.Should().Be("\"127.0.0.1\"");
    }

    [Fact]
    public void Other_values_in_an_object_slot_are_written_by_their_own_converter()
    {
        // Arrange
        var values = new Dictionary<string, object> { ["number"] = 5, ["text"] = "10.0.0.1" };

        // Act
        var json = JsonSerializer.Serialize(values, s_jsonOptions);

        // Assert
        json.Should().Be("{\"number\":5,\"text\":\"10.0.0.1\"}");
    }

    [Fact]
    public void Address_string_in_an_object_slot_is_not_read_as_address()
    {
        // Arrange
        const string AddressJson = "\"10.0.0.1\"";

        // Act
        var deserialized = JsonSerializer.Deserialize<object>(AddressJson, s_jsonOptions);

        // Assert
        deserialized.Should().BeOfType<JsonElement>();
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("\" \"")]
    [InlineData("\"not-an-address\"")]
    [InlineData("\"256.0.0.1\"")]
    [InlineData("5")]
    [InlineData("true")]
    [InlineData("{}")]
    [InlineData("[\"10.0.0.1\"]")]
    public void Reading_a_non_string_or_unparsable_value_throws(string json)
    {
        // Arrange
        // Act
        var act = () => JsonSerializer.Deserialize<IPAddress>(json, s_jsonOptions);

        // Assert
        act.Should().Throw<JsonException>();
    }
}
