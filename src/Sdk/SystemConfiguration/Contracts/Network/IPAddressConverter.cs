using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents an <see cref="IPAddressConverter"/>.
/// </summary>
public class IPAddressConverter : JsonConverter<IPAddress>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsAssignableFrom(typeof(IPAddress));

    /// <inheritdoc/>
    public override IPAddress Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string as the JSON token type.");

        if (string.IsNullOrWhiteSpace(reader.GetString()))
            throw new JsonException($"IPAddress '{reader.GetString()}' can not be empty.");

        if (IPAddress.TryParse(reader.GetString(), out var ipAddress))
            return ipAddress;
        else
            throw new JsonException($"Invalid IP address format: '{reader.GetString()}'");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        IPAddress value,
        JsonSerializerOptions options)
    {
        var ipAddressString = value is null
            ? string.Empty
            : value.ToString();

        writer.WriteStringValue(ipAddressString);
    }
}
