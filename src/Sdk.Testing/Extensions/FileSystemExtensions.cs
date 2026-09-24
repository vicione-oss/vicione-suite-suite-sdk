using System.IO.Abstractions;

namespace Sdk.Testing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IFileSystem"/> to simplify file system interactions in tests.
/// </summary>
public static class FileSystemExtensions
{
    extension(IFileSystem fileSystem)
    {
        /// <summary>
        /// Returns the nearest directory, from the current directory upwards, that contains <c>readme.md</c> or
        /// <c>changelog.md</c> (case-insensitive); an empty string if none does.
        /// </summary>
        public string GetRepositoryRootPath()
            => fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.md");

        /// <summary>
        /// Returns the nearest directory, from the current directory upwards, containing a file that matches
        /// <paramref name="fileFilter"/> and is named like any of <paramref name="fileNames"/> (case-insensitive);
        /// an empty string if none does. The file-system root is not searched.
        /// </summary>
        public string GetPathContaining(IList<string> fileNames, string fileFilter)
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
}
