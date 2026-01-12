using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdk.SystemConfiguration.Contracts.Network;

/// <summary>
/// Represents an <see cref="DateTimeOffsetConverter"/>.
/// </summary>
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert == typeof(DateTimeOffset?) ||
           typeToConvert == typeof(DateTimeOffset);

    /// <inheritdoc/>
    public override DateTimeOffset? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string as the JSON token type.");

        var dateTimeOffsetString = reader.GetString();

        if (string.IsNullOrWhiteSpace(dateTimeOffsetString))
            throw new JsonException($"DateTimeOffset '{dateTimeOffsetString}' can not be empty.");

        if (!dateTimeOffsetString.Contains('T', StringComparison.Ordinal))
        {
            throw new JsonException(
                $"DateTimeOffset '{dateTimeOffsetString}' is missing 'T' separator " +
                "(e.g. '2024-10-30T01:23:45+0200').");
        }

        if (DateTimeOffset.TryParse(reader.GetString().AsSpan(), out var dateTime))
        {
            return dateTime;
        }
        else
        {
            throw new JsonException(
                $"DateTimeOffset '{dateTimeOffsetString}' has to be in DateTimeOffset format " +
                "(e.g. '2024-10-30T01:23:45+0200').");
        }
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset? value,
        JsonSerializerOptions options)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            // Creates an ISO-8601 conform format (e.g.: "2024-12-04T10:15:42+0200")
            //
            // All non-ASCII characters will get escaped by the serializer by default (like "+"),
            // so for above example the string would be "2024-12-04T10:15:42\u002B0200".

#pragma warning disable CA1305 // Specify IFormatProvider
            writer.WriteStringValue(dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ssK"));
#pragma warning restore CA1305 // Specify IFormatProvider
        }
        else
        {
            throw new JsonException($"Invalid DateTimeOffset value: '{value}'.");
        }
    }
}

