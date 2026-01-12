using System.IO.Abstractions;
using System.Text.Json;
using AwesomeAssertions;
using NSubstitute;
using Sdk.Modules;
using Xunit;

namespace Sdk.Testing.Tests;

public sealed class MetadataValidatorTests
{
    private const string MetadataFilePath = "/repo/metadata.json";

    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();

    [Fact]
    public void Should_validate_serialization_and_sdk_version()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion();
        SetupMetadataFile(sdkVersion.ToString());

        // Act + Assert
        MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath);
    }

    [Fact]
    public void Should_validate_dependency_is_public()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion().ToString();
        var assembly = typeof(ModuleMetadata).Assembly;
        var metadata = new ModuleMetadata
        {
            Version = "1.0.0",
            Name = "Test",
            MinSuiteSdkVersion = sdkVersion,
            Dependencies =
            [
                new () { Name = "Test", Version = "1.0.0" },
            ],
        };

        SetupMetadataFile(sdkVersion, metadata);

        // Act 
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath, assembly);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*is*no*public*module*library*");
    }

    [Fact]
    public void Should_validate_dependency_version_matches()
    {
        // todo - to really test this behavior we'll need a Test.Public.dll to be referenced
    }

    [Fact]
    public void Should_throw_if_serialization_fails()
    {
        // Arrange
        _fileSystem.File.Exists(MetadataFilePath).Returns(true);
        _fileSystem.File.ReadAllText(MetadataFilePath).Returns("}");

        // Act 
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Should_throw_if_sdk_version_does_not_match()
    {
        // Arrange
        SetupMetadataFile("0.47.11");

        // Act 
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*does*not*match*SDK*");
    }

    [Fact]
    public void Should_throw_if_metadata_sdk_version_is_invalid()
    {
        // Arrange
        SetupMetadataFile("x.A.z");

        // Act 
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*invalid*version*");
    }

    [Fact]
    public void Should_throw_if_metadata_path_does_not_exist()
    {
        // Arrange
        var wrongPath = "./metadata.json";

        // Act 
        var act = () => MetadataValidator.ValidateMetadata(wrongPath);

        // Assert
        act.Should().Throw<FileNotFoundException>();
    }

    private void SetupMetadataFile(string minSuiteVersion, ModuleMetadata? metadata = null)
    {
        metadata ??= new ModuleMetadata
        {
            Version = "1.0.0",
            Name = "Test",
            MinSuiteSdkVersion = minSuiteVersion,
        };

        _fileSystem.File.Exists(MetadataFilePath).Returns(true);
        _fileSystem.File.ReadAllText(MetadataFilePath).Returns(JsonSerializer.Serialize(metadata));
    }
}
