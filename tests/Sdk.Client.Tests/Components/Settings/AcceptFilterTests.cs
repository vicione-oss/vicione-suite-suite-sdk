using AwesomeAssertions;
using Sdk.Client.Components.Settings;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class AcceptFilterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(" , ")]
    [InlineData("png, invalid, /")]
    public void Should_accept_any_file_when_accept_has_no_valid_token(string? accept)
    {
        // Act
        var result = AcceptFilter.Matches(accept, "photo.jpg", "image/jpeg");

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(".png,.jpg")]
    [InlineData(".png, .jpg")]
    [InlineData(" .png , .JPG ")]
    [InlineData(".jpg")]
    public void Should_match_file_extension_ignoring_whitespace_and_case(string accept)
    {
        // Act
        var result = AcceptFilter.Matches(accept, "photo.jpg", "");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_match_multi_part_file_extension()
    {
        // Act
        var result = AcceptFilter.Matches(".tar.gz", "backup.tar.gz", "application/gzip");

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("image/*")]
    [InlineData("IMAGE/*")]
    [InlineData("image/jpeg")]
    [InlineData(".png, image/jpeg")]
    public void Should_match_mime_type_and_wildcard(string accept)
    {
        // Act
        var result = AcceptFilter.Matches(accept, "photo", "image/jpeg");

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(".png, .gif", "photo.jpg", "image/jpeg")]
    [InlineData("image/png", "photo.jpg", "image/jpeg")]
    [InlineData("video/*", "photo.jpg", "image/jpeg")]
    [InlineData("image/*", "photo.jpg", "")]
    [InlineData(".jpg", "photo.jpg.exe", "application/octet-stream")]
    public void Should_reject_file_that_matches_no_token(string accept, string fileName, string contentType)
    {
        // Act
        var result = AcceptFilter.Matches(accept, fileName, contentType);

        // Assert
        result.Should().BeFalse();
    }
}
