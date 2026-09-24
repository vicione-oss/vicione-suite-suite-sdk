using System.IO.Abstractions;
using System.Reflection;
using System.Reflection.Emit;
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
    public void Should_accept_public_dependency_with_matching_version()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion().ToString();
        var dependency = CreateAssembly("Test.Dependency.Public", new Version(1, 2, 3, 4));
        SetupMetadataFile(sdkVersion, CreateMetadata(sdkVersion, "Test.Dependency", "1.2.3"));

        // Act
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath, dependency);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_throw_if_dependency_version_does_not_match()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion().ToString();
        var dependency = CreateAssembly("Test.Dependency.Public", new Version(1, 2, 4));
        SetupMetadataFile(sdkVersion, CreateMetadata(sdkVersion, "Test.Dependency", "1.2.3"));

        // Act
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath, dependency);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Test.Dependency*1.2.3*does*not*match*1.2.4*");
    }

    [Fact]
    public void Should_throw_if_dependency_is_missing_in_metadata()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion().ToString();
        var dependency = CreateAssembly("Test.Dependency.Public", new Version(1, 2, 3));
        SetupMetadataFile(sdkVersion, CreateMetadata(sdkVersion, "Other.Module", "1.2.3"));

        // Act
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath, dependency);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Test.Dependency*missing*");
    }

    [Fact]
    public void Should_report_the_invalid_dependency_version()
    {
        // Arrange
        var sdkVersion = MetadataValidator.GetSdkVersion().ToString();
        var dependency = CreateAssembly("Test.Dependency.Public", new Version(1, 2, 3));
        SetupMetadataFile(sdkVersion, CreateMetadata(sdkVersion, "Test.Dependency", "not-a-version"));

        // Act
        var act = () => MetadataValidator.ValidateMetadata(_fileSystem, MetadataFilePath, dependency);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*invalid*version*not-a-version*");
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
            .WithMessage("*invalid*version*x.A.z*");
    }

    [Fact]
    public void Should_throw_if_metadata_path_does_not_exist()
    {
        // Arrange
        const string WrongPath = "./metadata.json";

        // Act
        var act = () => MetadataValidator.ValidateMetadata(WrongPath);

        // Assert
        act.Should().Throw<FileNotFoundException>();
    }

    // A dynamic assembly carries exactly the name and version the validator reads, without shipping a *.Public.dll.
    private static AssemblyBuilder CreateAssembly(string name, Version version)
        => AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name) { Version = version }, AssemblyBuilderAccess.Run);

    private static ModuleMetadata CreateMetadata(string sdkVersion, string dependencyName, string dependencyVersion)
        => new()
        {
            Version = "1.0.0",
            Name = "Test",
            MinSuiteSdkVersion = sdkVersion,
            Dependencies =
            [
                new() { Name = dependencyName, Version = dependencyVersion },
            ],
        };

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
