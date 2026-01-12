namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents an artifact available through an <see cref="IArtifactRepository"/>.
/// </summary>
public interface IArtifact
{
    /// <summary>
    /// Gets the date and time when the artifact was last modified.
    /// </summary>
    /// <remarks>
    /// Is <see langword="null"/> if the information is not available.
    /// </remarks>
    DateTimeOffset? Modified { get; }

    /// <summary>
    /// Gets the name of the artifact file.
    /// </summary>
    /// <example><c>0.18.2-win-x64_0.30.0.json</c></example>
    string Name { get; }

    /// <summary>
    /// Gets the relative path of the artifact within the repository.
    /// </summary>
    /// <example><c>/functionblocks/ViciOne.SomeFb</c></example>
    string Path { get; }

    /// <summary>
    /// Gets the name of the repository in which the artifact resides.
    /// </summary>
    /// <example><c>vicione-suite</c></example>
    string Repository { get; }

    /// <summary>
    /// Gets the size of the artifact in bytes.
    /// </summary>
    /// <remarks>
    /// Is <see langword="null"/> if size information is not available.
    /// </remarks>
    long? Size { get; }

    /// <summary>
    /// Gets the kind of artifact
    /// </summary>
    ArtifactKind Kind { get; }

    /// <summary>
    /// Gets the unique source key identifier for the artifact.
    /// It is used to reference the source repository the artifact belongs to.
    /// </summary>
    string SourceKey { get; }

    /// <summary>
    /// Gets the checksum used to verify the artifact’s integrity.
    /// </summary>
    /// <remarks>
    /// Is <see langword="null"/> if no checksum is available.
    /// </remarks>
    IArtifactChecksum? Checksum { get; }
}
