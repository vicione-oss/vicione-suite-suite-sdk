namespace Sdk.Backend.ArtifactApi;

/// <summary>
/// Represents checksum information for an artifact.
/// </summary>
public interface IArtifactChecksum
{
    string? Sha1 { get; set; }
    string? Sha256 { get; set; }
    string? Sha512 { get; set; }
    string? Md5 { get; set; }
}
