namespace Sdk.Backend.ArtifactApi;

/// <summary>
/// ArtifactItem represents a file available through <see cref="IArtifactQueryApi"/>
/// </summary>
public interface IArtifactItem
{
    /// <summary>
    /// Datetime of last modification
    /// </summary>
    DateTimeOffset? Modified { get; set; }

    /// <summary>
    /// e.g. 0.18.2-win-x64_0.30.0.json
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Relative path to the artifact like /functionblocks/ViciOne.SomeFb
    /// </summary>
    string Path { get; set; }

    /// <summary>
    /// Used repository name like 'vicione-suite'
    /// </summary>
    string Repo { get; set; }

    /// <summary>
    /// Size in bytes
    /// </summary>
    long? Size { get; set; }

    /// <summary>
    /// Checksum information for verifying integrity. Optional.
    /// </summary>
    IArtifactChecksum? Checksum { get; set; }
}
