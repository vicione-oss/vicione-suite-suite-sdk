namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents checksum information for an artifact.
/// </summary>
public interface IArtifactChecksum
{
    /// <summary>
    /// Gets the SHA-1 checksum of the artifact.
    /// </summary>
    string? Sha1 { get; }

    /// <summary>
    /// Gets the SHA-256 checksum of the artifact.
    /// </summary>
    string? Sha256 { get; }

    /// <summary>
    /// Gets the SHA-512 checksum of the artifact.
    /// </summary>
    string? Sha512 { get; }

    /// <summary>
    /// Gets the MD5 checksum of the artifact.
    /// </summary>
    string? Md5 { get; }
}
