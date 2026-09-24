namespace Sdk.Client.Models;

/// <summary>
/// The progress of a stream upload.
/// </summary>
public interface IStreamUploadProgress
{
    /// <summary>
    /// Gets the path the upload targets.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the name of the uploaded file.
    /// </summary>
    string Filename { get; }

    /// <summary>
    /// Gets the path of the stored file; <see langword="null"/> until it is known.
    /// </summary>
    string? DestinationFile { get; }

    /// <summary>
    /// Gets the total number of bytes to upload.
    /// </summary>
    long BytesTotal { get; }

    /// <summary>
    /// Gets the number of bytes uploaded so far.
    /// </summary>
    long BytesUploaded { get; }
}
