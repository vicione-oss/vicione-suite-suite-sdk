namespace Sdk.Backend.IO;

/// <summary>
/// Writes files atomically so a crash or power-loss mid-write can never leave a
/// truncated or corrupt file on disk.
/// </summary>
public interface IAtomicFileWriter
{
    /// <summary>
    /// Writes a file atomically: the content is written to a temporary sibling file first
    /// and then atomically moved into place, keeping the previous valid file intact until
    /// the new one is fully persisted. Any missing parent directories are created.
    /// </summary>
    /// <param name="filePath">The destination path of the file to write.</param>
    /// <param name="writeContent">A callback that writes the file content to the provided stream.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteAsync(string filePath, Func<Stream, Task> writeContent, CancellationToken cancellationToken = default);
}
