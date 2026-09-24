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
    /// Gets the result range per source; available when the query used a limit or offset.
    /// </summary>
    IReadOnlyCollection<IArtifactQueryRange>? Ranges { get; }

    /// <summary>
    /// Gets the errors of sources whose query failed; <see cref="Artifacts"/> still holds the results of the other sources.
    /// </summary>
    IReadOnlyCollection<IArtifactQueryError>? Errors { get; }
}
