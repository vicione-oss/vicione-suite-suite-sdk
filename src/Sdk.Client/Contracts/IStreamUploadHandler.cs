using Sdk.Client.Models;

namespace Sdk.Client.Contracts;

/// <summary>
/// Handler for uploading a stream to a file, T is used as a marker to identifiy the handler for a specific upload control
/// </summary>
public interface IStreamUploadHandler<T>
{
    /// <summary>
    /// Raised to notify about progress in <see cref="Execute(Stream, string, CancellationToken)"/>
    /// </summary>
    Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

    /// <summary>
    /// Uploads the given <paramref name="stream"/> to a file named <paramref name="filename"/>
    /// </summary>
    /// <param name="stream">Stream to upload</param>
    /// <param name="filename">Name of the file resulting from the upload</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <exception cref="OperationCanceledException" />
    /// <returns>async Task</returns>
    Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default);
}
