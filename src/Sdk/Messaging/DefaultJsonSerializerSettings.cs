using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdk.Messaging;

public static class DefaultJsonSerializerSettings
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { CreateDateTimeConverter() }
    };

    public static JsonConverter<DateTime> CreateDateTimeConverter()
        => new DateTimeConverter();

    private sealed class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTime().ToUniversalTime();

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime());
        }
    }
}
