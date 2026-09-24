namespace Sdk.Client.Models;

/// <summary>
/// The result of a successful upload.
/// </summary>
/// <param name="destinationFile">The path of the stored file.</param>
public class StreamUploadSuccessResult(string destinationFile) : IStreamUploadResult
{
    /// <summary>
    /// Gets the path of the stored file.
    /// </summary>
    public string DestinationFile => destinationFile;
}
