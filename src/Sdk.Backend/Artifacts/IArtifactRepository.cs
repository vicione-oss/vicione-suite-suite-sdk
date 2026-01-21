namespace Sdk.Backend.Artifacts;

/// <summary>
/// Provides methods for querying, downloading, and managing artifacts through the ViciOne.Suite infrastructure.
/// </summary>
public interface IArtifactRepository
{
    /// <summary>
    /// Executes an artifact query using Artifact Query Language (AQL) to retrieve matching artifacts.
    /// </summary>
    Task<IArtifactQueryResult> Query(string aqlQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the specified artifact as a <see cref="Stream"/>.
    /// </summary>
    Task<Stream> Download(IArtifact artifact, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the specified artifact and saves it to the given local file path.
    /// </summary>
    Task DownloadToFile(IArtifact artifact, string targetFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads and extracts an archive artifact into the specified target directory.
    /// </summary>
    Task DownloadAndExtract(IArtifact artifact, string targetFolderPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new instance of an <see cref="IArtifactQueryBuilder"/> to assist in building artifact queries.
    /// </summary>
    IArtifactQueryBuilder CreateQueryBuilder();

    /// <summary>
    /// Gets the absolute URI that can be used to download the specified artifact directly.
    /// </summary>
    Uri GetDownloadUri(IArtifact artifact);

    /// <summary>
    /// Gets a collection of available artifact source keys. These keys are used to identify artifact sources internally.
    /// </summary>
    IEnumerable<string> GetSourceKeys();
}
