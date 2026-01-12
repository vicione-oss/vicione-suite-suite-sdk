namespace Sdk.Backend.ArtifactApi;

public interface IArtifactQueryResult
{
    /// <summary>
    /// Results from query with <see cref="IArtifactQueryApi.ExecuteQuery(string, CancellationToken)" />
    /// </summary>
    public IReadOnlyCollection<IArtifactItem> Artifacts { get; }
}
