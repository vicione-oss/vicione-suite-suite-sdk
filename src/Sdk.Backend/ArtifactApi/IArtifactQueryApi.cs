namespace Sdk.Backend.ArtifactApi;


/// <summary>
/// API to access artifacts (files, images, etc.) via the ViciOne.Suite infrastructure
/// </summary>
public interface IArtifactQueryApi
{
    /// <summary>
    /// Executes an artifact query against the ViciOne.Suite infrastructure to retrieve matching artifacts.
    /// </summary>
    /// <param name="aqlQuery">The AQL (Artifact Query Language) string to execute.</param>
    Task<IArtifactQueryResult> ExecuteQuery(string aqlQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the specified artifact as a stream.
    /// </summary>
    /// <param name="artifact">The artifact to download</param>
    Task<Stream> DownloadStream(IArtifactItem artifact, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the specified artifact directly to a local file.
    /// </summary>
    /// <param name="artifact">Retrieved by <see cref="ExecuteQuery"/></param>    
    Task DownloadToFile(IArtifactItem artifact, string targetFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads and extracts an archive-type artifact into the specified target directory.
    /// </summary>
    /// <returns></returns>
    Task DownloadAndExtract(IArtifactItem artifact, string targetFolderPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new instance of an artifact query builder.
    /// </summary>
    IArtifactQueryBuilder CreateQueryBuilder();

    /// <summary>
    /// Creates an instance of IArtifactItem
    /// </summary>
    IArtifactItem CreateArtifactItem(string path, string name, long? size = null, DateTime? modified = null);

    /// <summary>
    /// Creates an absolute Uri with path to download the artifact
    /// </summary>
    Uri CreateDownloadUri(IArtifactItem artifact);
}
