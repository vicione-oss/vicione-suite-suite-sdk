using System.Text.Json;
using AwesomeAssertions;
using Sdk.Messaging;

namespace Sdk.Tests.Messaging;

public sealed class DefaultJsonSerializerSettingsTests
{
    public sealed class DateTimeConverterTests
    {
        [Fact]
        public void Should_serialize_datetime_as_utc()
        {
            // Arrange
            var date = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
            var wrapper = new DateTimeWrapper { Value = date };

            // Act
            var json = JsonSerializer.Serialize(wrapper, DefaultJsonSerializerSettings.Default);

            // Assert
            json.Should().Contain("2025-06-15T10:30:00Z");
        }

        [Fact]
        public void Should_deserialize_datetime_as_utc()
        {
            // Arrange
            const string Json = """{"Value":"2025-06-15T10:30:00Z"}""";

            // Act
            var result = JsonSerializer.Deserialize<DateTimeWrapper>(Json, DefaultJsonSerializerSettings.Default);

            // Assert
            result.Should().NotBeNull();
            result!.Value.Kind.Should().Be(DateTimeKind.Utc);
            result.Value.Should().Be(new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void Should_convert_local_datetime_to_utc_on_serialize()
        {
            // Arrange
            var localDate = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Local);
            var wrapper = new DateTimeWrapper { Value = localDate };

            // Act
            var json = JsonSerializer.Serialize(wrapper, DefaultJsonSerializerSettings.Default);
            var deserialized = JsonSerializer.Deserialize<DateTimeWrapper>(json, DefaultJsonSerializerSettings.Default);

            // Assert
            deserialized!.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Should_roundtrip_datetime_preserving_utc()
        {
            // Arrange
            var utcDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var wrapper = new DateTimeWrapper { Value = utcDate };

            // Act
            var json = JsonSerializer.Serialize(wrapper, DefaultJsonSerializerSettings.Default);
            var result = JsonSerializer.Deserialize<DateTimeWrapper>(json, DefaultJsonSerializerSettings.Default);

            // Assert
            result!.Value.Should().Be(utcDate);
            result.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        private sealed class DateTimeWrapper
        {
            public DateTime Value { get; set; }
        }
    }

    public sealed class DefaultOptionsTests
    {
        [Fact]
        public void Should_be_case_insensitive()
        {
            // Assert
            DefaultJsonSerializerSettings.Default.PropertyNameCaseInsensitive.Should().BeTrue();
        }

        [Fact]
        public void Should_ignore_null_when_writing()
        {
            // Arrange
            var obj = new NullableWrapper { Name = null, Value = "test" };

            // Act
            var json = JsonSerializer.Serialize(obj, DefaultJsonSerializerSettings.Default);

            // Assert
            json.Should().NotContain("Name");
            json.Should().Contain("Value");
        }

        private sealed class NullableWrapper
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
        }
    }
}

