namespace Sdk.Client.Models;

/// <summary>
/// The result of a failed upload.
/// </summary>
/// <param name="message">Describes why the upload failed.</param>
/// <param name="errorCode">An optional machine-readable code; <see langword="null"/> if there is none.</param>
public class StreamUploadErrorResult(string message, int? errorCode = null) : IStreamUploadResult
{
    /// <summary>
    /// Gets the description of why the upload failed.
    /// </summary>
    public string Message => message;

    /// <summary>
    /// Gets the machine-readable error code; <see langword="null"/> if there is none.
    /// </summary>
    public int? ErrorCode => errorCode;
}
