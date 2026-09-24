using Framework = System.IO.Path;

namespace Sdk.Testing;

/// <summary>
/// Creates a directory that is deleted, with its contents, on dispose. A relative path resolves against the
/// current directory, not the system temp directory.
/// </summary>
public sealed class TemporaryDirectory : IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Gets the directory path as passed to the constructor; relative unless an absolute path was given.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Creates a randomly named directory in the current directory.
    /// </summary>
    public TemporaryDirectory()
        : this(Framework.GetRandomFileName())
    { }

    /// <summary>
    /// Creates <paramref name="path"/>, or reuses it if it exists; it is deleted on dispose either way.
    /// </summary>
    public TemporaryDirectory(string path)
    {
        Path = path;
        Directory.CreateDirectory(Path);
    }

    /// <summary>
    /// Creates the nested directory <paramref name="pathParts"/> below <see cref="Path"/> and returns its path.
    /// </summary>
    public string CreateDirectory(params string[] pathParts)
    {
        var path = Framework.Combine(Path, Framework.Combine(pathParts));
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Creates an empty file below <see cref="Path"/>, creating missing parent directories, and returns its path.
    /// The last element of <paramref name="pathParts"/> is the file name; an existing file is truncated.
    /// </summary>
    public string CreateFile(params string[] pathParts)
    {
        var directoryPath = Framework.Combine(Path, Framework.Combine(pathParts[..^1]));
        Directory.CreateDirectory(directoryPath);
        var filePath = Framework.Combine(Path, Framework.Combine(pathParts));
        File.Create(filePath).Dispose();
        return filePath;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposedValue)
        {
            if (Directory.Exists(Path))
            {
                try
                {
                    Directory.Delete(Path, true);
                }
                catch
                {
                    // Best effort: a file the test still holds open must not fail the test run.
                }
            }
            _disposedValue = true;
        }
    }
}
