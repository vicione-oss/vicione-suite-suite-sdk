namespace Sdk.Backend.ArtifactApi;

/// <summary>
/// Fluent builder for constructing artifact queries against the ViciOne.Suite infrastructure.
/// </summary>
public interface IArtifactQueryBuilder
{
    /// <summary>
    /// Adds an 'AND' condition that the artifact name must match the specified regex pattern.
    /// </summary>
    /// <param name="pattern">A regex pattern to match artifact names.</param>    
    IArtifactQueryBuilder AndNameMatches(string pattern);

    /// <summary>
    /// Adds an 'AND' condition that the artifact name must match the specified regex pattern.
    /// </summary>
    /// <param name="pattern">A regex pattern to not match artifact names.</param>    
    IArtifactQueryBuilder AndNameNotMatches(string pattern);

    /// <summary>
    /// Adds an 'AND' condition that the artifact path must match the specified regex pattern.
    /// </summary>
    /// <param name="pattern">A regex pattern to match artifact names.</param>
    /// <returns>The current builder instance.</returns>
    IArtifactQueryBuilder AndPathMatches(string pattern);

    // Selection & sorting
    IArtifactQueryBuilder IncludeFields(params string[] includes);
    IArtifactQueryBuilder OrderBy(params string[] fields);
    IArtifactQueryBuilder OrderByDescending(params string[] fields);

    /// <summary>
    /// Builds the artifact query string in AQL format.
    /// </summary>
    /// <returns>The constructed query as a string.</returns>
    string BuildQueryString();
}
