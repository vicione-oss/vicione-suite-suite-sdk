namespace Sdk.Backend.Artifacts;

/// <summary>
/// Represents the result of a query performed against an artifact repository.
/// </summary>
public interface IArtifactQueryResult
{
    /// <summary>
    /// Gets the collection of artifacts returned by the query.
    /// </summary>
    IReadOnlyCollection<IArtifact> Artifacts { get; }

    /// <summary>
    /// Gets the range information for the query results, which is available if pagination features like limit or offset were used.
    /// </summary>
    IReadOnlyCollection<IArtifactQueryRange>? Ranges { get; }

    /// <summary>
    /// Error information for failed queries to a source. With multiple sources configured
    /// the query might still return Artifacts for all sources that returned results.
    /// </summary>
    IReadOnlyCollection<IArtifactQueryError>? Errors { get; }
}
