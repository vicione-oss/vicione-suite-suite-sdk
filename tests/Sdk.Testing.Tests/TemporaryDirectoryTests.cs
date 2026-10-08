using AwesomeAssertions;
using Xunit;

namespace Sdk.Testing.Tests;

public sealed class TemporaryDirectoryTests
{
    [Fact]
    public void Constructor_should_create_directory()
    {
        // Act
        using var tempDir = new TemporaryDirectory();

        // Assert
        Directory.Exists(tempDir.Path).Should().BeTrue();
    }

    [Fact]
    public void Constructor_with_name_should_create_named_directory()
    {
        // Arrange
        const string DirName = "test-temp-dir-" + nameof(Constructor_with_name_should_create_named_directory);

        // Act
        using var tempDir = new TemporaryDirectory(DirName);

        // Assert
        Directory.Exists(tempDir.Path).Should().BeTrue();
        tempDir.Path.Should().Be(DirName);
    }

    [Fact]
    public void Dispose_should_delete_directory()
    {
        // Arrange
        var tempDir = new TemporaryDirectory();
        var path = tempDir.Path;
        Directory.Exists(path).Should().BeTrue();

        // Act
        tempDir.Dispose();

        // Assert
        Directory.Exists(path).Should().BeFalse();
    }

    [Fact]
    public void Dispose_twice_should_not_throw()
    {
        // Arrange
        var tempDir = new TemporaryDirectory();

        // Act
        tempDir.Dispose();
        var act = tempDir.Dispose;

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateDirectory_should_create_subdirectory()
    {
        // Arrange
        using var tempDir = new TemporaryDirectory();

        // Act
        var subDirPath = tempDir.CreateDirectory("sub", "nested");

        // Assert
        Directory.Exists(subDirPath).Should().BeTrue();
        subDirPath.Should().Contain("sub");
        subDirPath.Should().Contain("nested");
    }

    [Fact]
    public void CreateFile_should_create_file_with_directories()
    {
        // Arrange
        using var tempDir = new TemporaryDirectory();

        // Act
        var filePath = tempDir.CreateFile("subdir", "test.txt");

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }
}

