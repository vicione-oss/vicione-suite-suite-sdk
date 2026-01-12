namespace Sdk.Backend.Artifacts;

/// <summary>
/// Fluent builder for constructing artifact queries.
/// </summary>
public interface IArtifactQueryBuilder
{
    /// <summary>
    /// Adds a condition that the artifact name must match the specified regular expression pattern.
    /// </summary>
    IArtifactQueryBuilder AndNameMatches(string pattern);

    /// <summary>
    /// Adds a condition that the artifact name must NOT match the specified regular expression pattern.
    /// </summary>
    IArtifactQueryBuilder AndNameNotMatches(string pattern);

    /// <summary>
    /// Adds a condition that the artifact path must match the specified regular expression pattern.
    /// </summary>
    IArtifactQueryBuilder AndPathMatches(string pattern);

    /// <summary>
    /// Specifies which fields to include in the query result.
    /// </summary>
    IArtifactQueryBuilder IncludeFields(params string[] includes);

    /// <summary>
    /// Specifies the fields by which to sort the query results in ascending order.
    /// </summary>
    IArtifactQueryBuilder OrderBy(params string[] fields);

    /// <summary>
    /// Specifies the fields by which to sort the query results in descending order.
    /// </summary>
    IArtifactQueryBuilder OrderByDescending(params string[] fields);

    /// <summary>
    /// Filters the query result to include only artifacts of the specified kind.
    /// </summary>
    IArtifactQueryBuilder FilterBy(ArtifactKind artifactKind);

    /// <summary>
    /// Adds a limit with an optional offset to the number of items returned by the query.
    /// </summary>
    IArtifactQueryBuilder Limit(int itemLimit, int offset = 0);

    /// <summary>
    /// Builds the final artifact query string in AQL format.
    /// </summary>
    string Build();
}
