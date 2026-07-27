namespace Sdk.Client.Contracts;

/// <summary>
/// Represents the options for configuring the stream upload handler.
/// </summary>
public sealed class StreamUploadHandlerOptions
{
    /// <summary>
    /// Customizes the directory that the file gets uploaded to.
    /// </summary>
    public Func<string, string>? PathTransform { get; set; }

    /// <summary>
    /// Customizes the name that the file gets once it is uploaded.
    /// </summary>
    public Func<string, string>? FilenameTransform { get; set; }
}
