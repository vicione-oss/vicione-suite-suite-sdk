namespace Sdk.Backend.Artifacts;

/// <summary>
/// Defines the kind of artifacts supported by the Artifact API.
/// </summary>
public enum ArtifactKind
{
    /// <summary>
    /// The artifact is a file.
    /// </summary>
    File,

    /// <summary>
    /// The artifact is a folder, which can contain other files and folders.
    /// </summary>
    Folder
}
