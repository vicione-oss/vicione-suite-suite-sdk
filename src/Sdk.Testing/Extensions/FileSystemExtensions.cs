using System.IO.Abstractions;

namespace Sdk.Testing.Extensions;

public static class FileSystemExtensions
{
    /// <summary>
    /// Returns first parent path containing readme.md and changelog.md (ordinal ignorec case) 
    /// starting at parent of Environment.CurrentDirectory
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <returns></returns>
    public static string GetRepositoryRootPath(this IFileSystem fileSystem)
        => fileSystem.GetPathContaining(["readme.md", "changelog.md"], "*.md");

    /// <summary>
    /// Returns first parent path containing given fileNames
    /// starting at parent of Environment.CurrentDirectory
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <param name="fileNames"></param>
    /// <param name="fileFilter"></param>
    /// <returns></returns>
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
