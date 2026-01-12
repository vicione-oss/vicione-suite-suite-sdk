using Framework = System.IO.Path;

namespace Sdk.Testing;

/// <summary>
/// Creates a temporary directory on the file system that is automatically deleted when disposed.
/// </summary>
public sealed class TemporaryDirectory : IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Gets the full path of the temporary directory.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporaryDirectory"/> class with a random directory name.
    /// </summary>
    public TemporaryDirectory()
        : this(Framework.GetRandomFileName())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporaryDirectory"/> class with a specific directory name.
    /// </summary>
    public TemporaryDirectory(string path)
    {
        Path = path;
        Directory.CreateDirectory(Path);
    }

    /// <summary>
    /// Creates a new directory within this temporary directory.
    /// </summary>
    public string CreateDirectory(params string[] pathParts)
    {
        var path = Framework.Combine(Path, Framework.Combine(pathParts));
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Creates a new, empty file within this temporary directory.
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
                    // Suppress exceptions during cleanup
                }
            }
            _disposedValue = true;
        }
    }
}
