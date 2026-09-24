namespace Sdk.Client.Contracts;

/// <summary>
/// Represents the options for configuring the stream upload handler.
/// </summary>
public sealed class StreamUploadHandlerOptions
{
    /// <summary>
    /// Gets or sets the function that transforms the directory the file is uploaded to; <see langword="null"/> keeps it.
    /// </summary>
    public Func<string, string>? PathTransform { get; set; }

    /// <summary>
    /// Gets or sets the function that transforms the name the uploaded file is stored under; <see langword="null"/> keeps it.
    /// </summary>
    public Func<string, string>? FilenameTransform { get; set; }
}
