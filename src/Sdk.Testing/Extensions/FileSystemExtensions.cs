using System.IO.Abstractions;

namespace Sdk.Testing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IFileSystem"/> to simplify file system interactions in tests.
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>
    /// Returns the first parent path containing 'readme.md' and 'changelog.md' (case-insensitive),
    /// starting the search from the parent of the current directory.
    /// </summary>
    public static string GetRepositoryRootPath(this IFileSystem fileSystem)
        => fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.md");

    /// <summary>
    /// Returns the first parent path containing any of the specified file names,
    /// starting the search from the parent of the current directory.
    /// </summary>
    public static string GetPathContaining(this IFileSystem fileSystem, IList<string> fileNames, string fileFilter)
    {
        for (var directory = fileSystem.DirectoryInfo.New(Environment.CurrentDirectory); directory.Parent is not null; directory = directory.Parent)
        {
            if (directory.EnumerateFiles(fileFilter)
                .Any(x => fileNames.Any(fn => x.Name.Equals(fn, StringComparison.OrdinalIgnoreCase))))
            {
                return directory.FullName;
            }
        }

        return string.Empty;
    }
}
