using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdk.Messaging;

/// <summary>
/// Provides default, shared <see cref="JsonSerializerOptions"/> for messaging and serialization.
/// </summary>
public static class DefaultJsonSerializerSettings
{
    /// <summary>
    /// Gets the default, pre-configured <see cref="JsonSerializerOptions"/> instance.
    /// </summary>
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { CreateDateTimeConverter() }
    };

    /// <summary>
    /// Creates a new instance of the custom <see cref="DateTime"/> JSON converter.
    /// </summary>
    public static JsonConverter<DateTime> CreateDateTimeConverter()
        => new DateTimeConverter();

    /// <summary>
    /// A private JSON converter that ensures all DateTime values are handled as UTC.
    /// </summary>
    private sealed class DateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Reads a JSON string and converts it to a UTC <see cref="DateTime"/>.
        /// </summary>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTime().ToUniversalTime();

        /// <summary>
        /// Writes a <see cref="DateTime"/> value as a UTC string to JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToUniversalTime());
    }
}
