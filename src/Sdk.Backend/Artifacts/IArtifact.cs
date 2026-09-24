namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents an artifact available through an <see cref="IArtifactRepository"/>.
/// </summary>
public interface IArtifact
{
    /// <summary>
    /// Gets when the artifact was last modified; <see langword="null"/> if unknown.
    /// </summary>
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
    /// Gets the size of the artifact in bytes; <see langword="null"/> if unknown.
    /// </summary>
    long? Size { get; }

    /// <summary>
    /// Gets whether the artifact is a file or a folder.
    /// </summary>
    ArtifactKind Kind { get; }

    /// <summary>
    /// Gets the key of the configured source the artifact comes from; see <see cref="IArtifactRepository.GetSourceKeys"/>.
    /// </summary>
    string SourceKey { get; }

    /// <summary>
    /// Gets the checksums to verify the artifact's integrity with; <see langword="null"/> if none are available.
    /// </summary>
    IArtifactChecksum? Checksum { get; }
}
