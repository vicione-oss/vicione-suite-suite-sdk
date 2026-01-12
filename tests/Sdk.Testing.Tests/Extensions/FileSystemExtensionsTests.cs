using System.IO.Abstractions.TestingHelpers;
using AwesomeAssertions;
using Sdk.Testing.Extensions;
using Xunit;

namespace Sdk.Testing.Tests.Extensions;

public class FileSystemExtensionsTests
{
    private static MockDirectoryInfo SetupRepositoryDirectory(MockFileSystem fileSystem)
    {
        var current = new MockDirectoryInfo(fileSystem, Environment.CurrentDirectory);
        var currentParent = new MockDirectoryInfo(fileSystem, current.Parent.FullName);
        var parent = new MockDirectoryInfo(fileSystem, currentParent.Parent.FullName);
        var brother = new MockDirectoryInfo(fileSystem, Path.Combine(currentParent.Parent.FullName, "Brother"));
        var repoRoot = new MockDirectoryInfo(fileSystem, parent.Parent.FullName);

        fileSystem.AddDirectory(current);
        fileSystem.AddDirectory(currentParent);
        fileSystem.AddDirectory(parent);
        fileSystem.AddDirectory(brother);
        fileSystem.AddDirectory(repoRoot);

        return repoRoot;
    }

    public class GetRepositoryRootPath : FileSystemExtensionsTests
    {
        [Fact]
        public void Should_return_repository_root_path_if_possible()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var repoRoot = SetupRepositoryDirectory(fileSystem);

            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "readme.md")));
            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "changelog.md")));

            // Act
            var rootPath = fileSystem.GetRepositoryRootPath();

            // Assert
            rootPath.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Should_return_empty_string_if_root_path_does_not_exist()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var repoRoot = SetupRepositoryDirectory(fileSystem);

            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "readmes.md")));
            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "log.txt")));

            // Act
            var rootPath = fileSystem.GetRepositoryRootPath();

            // Assert
            rootPath.Should().BeNullOrEmpty();
        }
    }

    public class GetPathContaining : FileSystemExtensionsTests
    {
        [Fact]
        public void Should_return_path_if_it_contains_file_names()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var repoRoot = SetupRepositoryDirectory(fileSystem);

            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "readme.md")));
            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "changelog.md")));

            // Act
            var containingPath = fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.md");

            // Assert
            containingPath.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Should_return_empty_string_if_file_names_cant_be_found()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var repoRoot = SetupRepositoryDirectory(fileSystem);

            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "readmes.md")));
            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "log.txt")));

            // Act
            var containingPath = fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.md");

            // Assert
            containingPath.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Should_return_empty_string_if_file_names_dont_match_filter()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var repoRoot = SetupRepositoryDirectory(fileSystem);

            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "readme.md")));
            fileSystem.AddEmptyFile(new MockFileInfo(fileSystem, Path.Combine(repoRoot.FullName, "changelog.md")));

            // Act
            var containingPath = fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.txt");

            // Assert
            containingPath.Should().BeNullOrEmpty();
        }
    }
}
