using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents an <see cref="IPAddressListConverter"/>.
/// </summary>
public class IPAddressListConverter : JsonConverter<List<IPAddress>>
{
    /// <inheritdoc/>
    public override List<IPAddress> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array as the JSON token type.");

        var ipAddresses = new List<IPAddress>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string as the JSON token type.");

            if (IPAddress.TryParse(reader.GetString(), out var ipAddress))
                ipAddresses.Add(ipAddress);
            else
                throw new JsonException($"Invalid IP address format: {reader.GetString()}");
        }

        return ipAddresses;
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        List<IPAddress> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var ipAddress in value)
        {
            writer.WriteStringValue(ipAddress.ToString());
        }

        writer.WriteEndArray();
    }
}
