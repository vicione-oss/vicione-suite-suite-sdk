using Sdk.Client.Models;

namespace Sdk.Client.Contracts;

/// <summary>
/// Uploads a stream to a file in a module's workspace; <typeparamref name="T"/> is a marker that ties the handler to one upload control.
/// </summary>
public interface IStreamUploadHandler<T>
{
    /// <summary>
    /// Gets or sets the callback that receives progress while <see cref="Execute(Stream, string, CancellationToken)"/> runs.
    /// </summary>
    Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

    /// <summary>
    /// Uploads <paramref name="stream"/> to a file named <paramref name="filename"/>.
    /// </summary>
    /// <returns>A <see cref="StreamUploadSuccessResult"/> or a <see cref="StreamUploadErrorResult"/>.</returns>
    /// <exception cref="OperationCanceledException">Thrown if <paramref name="cancellationToken"/> is canceled.</exception>
    Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default);
}
