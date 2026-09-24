namespace Sdk.Backend.IO;

/// <summary>
/// Writes files atomically so a crash or power-loss mid-write can never leave a
/// truncated or corrupt file on disk.
/// </summary>
public interface IAtomicFileWriter
{
    /// <summary>
    /// Writes <paramref name="filePath"/> atomically: <paramref name="writeContent"/> fills a temporary sibling file, which
    /// is moved into place once fully persisted, so the previous file stays intact until then. Missing parent directories
    /// are created.
    /// </summary>
    Task WriteAsync(string filePath, Func<Stream, Task> writeContent, CancellationToken cancellationToken = default);
}
