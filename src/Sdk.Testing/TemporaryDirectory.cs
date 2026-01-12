using Framework = System.IO.Path;

namespace Sdk.Testing;

public sealed class TemporaryDirectory : IDisposable
{
    private bool _disposedValue;

    public string Path { get; }

    public TemporaryDirectory()
        : this(Framework.GetRandomFileName())
    { }

    public TemporaryDirectory(string path)
    {
        Path = path;
        Directory.CreateDirectory(Path);
    }

    public string CreateDirectory(params string[] pathParts)
    {
        var path = Framework.Combine(Path, Framework.Combine(pathParts));
        Directory.CreateDirectory(path);
        return path;
    }

    public string CreateFile(params string[] pathParts)
    {
        var directoryPath = Framework.Combine(Path, Framework.Combine(pathParts[..^1]));
        Directory.CreateDirectory(directoryPath);
        var filePath = Framework.Combine(Path, Framework.Combine(pathParts));
        File.Create(filePath).Dispose();
        return filePath;
    }

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
                }
            }
            _disposedValue = true;
        }
    }
}
